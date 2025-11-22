using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager :
    IBitBufferCreator<RawBitBuffer, HitBoxGadget>,
    IBitBufferCreator<RawBitBuffer, GadgetBase>,
    IBitBufferCreator<ArrayBitBuffer, GadgetTrigger>,
    IPerfectHasher<GadgetBehaviour>,
    IItemManager<GadgetBase>,
    IInitialisable,
    IDisposable
{
    private readonly GadgetBase[] _allGadgets;
    private readonly GadgetTrigger[] _allTriggers;
    private readonly GadgetBehaviour[] _allBehaviours;
    private readonly BitArraySet<GadgetManager, ArrayBitBuffer, GadgetTrigger> _indeterminateTriggers;
    private readonly SimpleList<CauseAndEffectData> _causeAndEffectData = new(256);

    private const int RequiredNumberOfGadgetBitSets =
        1 + // spacial hash grid
        2; // gadget sets

    private readonly RawArray _gadgetByteBuffer;
    private int _bitArrayBufferUsageCount = RequiredNumberOfGadgetBitSets;

    private readonly HitBoxGadgetSpacialHashGrid _hitBoxGadgetSpacialHashGrid;
    private readonly GadgetSet _fastForwardGadgets;
    private readonly GadgetSet _gadgetsToReEvaluate;

    public ReadOnlySpan<GadgetBase> AllItems => new(_allGadgets);
    public ReadOnlySpan<GadgetBehaviour> AllBehaviours => new(_allBehaviours);
    public GadgetBase GetGadget(int gadgetId) => _allGadgets[gadgetId];

    public GadgetManager(
        GadgetBase[] allGadgets,
        GadgetTrigger[] allTriggers,
        GadgetBehaviour[] allBehaviours,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        this.AssertUniqueIds(new ReadOnlySpan<GadgetBase>(allGadgets));
        Array.Sort(allGadgets, this);
        _allGadgets = allGadgets;

        this.AssertUniqueIds(new ReadOnlySpan<GadgetTrigger>(allTriggers));
        Array.Sort(allTriggers, this);
        _allTriggers = allTriggers;

        this.AssertUniqueIds(new ReadOnlySpan<GadgetBehaviour>(allBehaviours));
        Array.Sort(allBehaviours, this);
        _allBehaviours = allBehaviours;

        _indeterminateTriggers = new BitArraySet<GadgetManager, ArrayBitBuffer, GadgetTrigger>(this);

        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_allGadgets.Length);
        _gadgetByteBuffer = Helpers.AllocateBuffer<uint>(bitBufferLength * RequiredNumberOfGadgetBitSets);

        _hitBoxGadgetSpacialHashGrid = new HitBoxGadgetSpacialHashGrid(
            this,
            EngineConstants.GadgetPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _fastForwardGadgets = new GadgetSet(this);
        _gadgetsToReEvaluate = new GadgetSet(this);
    }

    public void Initialise()
    {
        foreach (var gadget in _allGadgets)
        {
            if (gadget.IsFastForward)
            {
                _fastForwardGadgets.Add(gadget);
            }

            if (gadget is HitBoxGadget hitBoxGadget)
            {
                _hitBoxGadgetSpacialHashGrid.AddItem(hitBoxGadget);
            }
        }
    }

    public void ResetForNewTick()
    {
        _gadgetsToReEvaluate.Clear();
        _causeAndEffectData.Clear();
        _indeterminateTriggers.Fill();

        foreach (var trigger in _allTriggers)
        {
            trigger.Reset();
        }

        foreach (var behaviour in _allBehaviours)
        {
            behaviour.Reset();
        }
    }

    public void Tick(bool isMajorTick)
    {
        if (isMajorTick)
        {
            TickAllGadgets();
        }
        else
        {
            TickFastForwardGadgets();
        }

        for (var i = 0; i < _causeAndEffectData.Count; i++)
        {
            var causeAndEffectDatum = _causeAndEffectData[i];
            var behaviour = _allBehaviours[causeAndEffectDatum.GadgetBehaviourId];
            behaviour.PerformBehaviour(causeAndEffectDatum.TriggerData);
        }

        FlagIndeterminateTriggersAsNotTriggered();
        ReEvaluateGadgets();
    }

    private void TickAllGadgets()
    {
        foreach (var gadget in _allGadgets)
        {
            gadget.Tick();
        }
    }

    private void TickFastForwardGadgets()
    {
        foreach (var gadget in _fastForwardGadgets)
        {
            gadget.Tick();
        }
    }

    public void FlagGadgetForReEvaluation(GadgetBase gadget)
    {
        _gadgetsToReEvaluate.Add(gadget);
    }

    private void ReEvaluateGadgets()
    {
        foreach (var gadget in _gadgetsToReEvaluate)
        {
            gadget.Tick();
        }
    }

    public void RegisterCauseAndEffectData(GadgetBehaviour behaviour)
    {
        _causeAndEffectData.Add(new CauseAndEffectData(behaviour.Id, 0));
    }

    public void RegisterCauseAndEffectData(GadgetBehaviour behaviour, int payload)
    {
        _causeAndEffectData.Add(new CauseAndEffectData(behaviour.Id, payload));
    }

    public void MarkTriggerAsEvaluated(GadgetTrigger gadgetTrigger)
    {
        _indeterminateTriggers.Remove(gadgetTrigger);
    }

    private void FlagIndeterminateTriggersAsNotTriggered()
    {
        foreach (var trigger in _indeterminateTriggers)
        {
            trigger.DetermineTrigger(false);
        }
    }

    public void GetAllGadgetsNearPosition(Point levelPosition, out GadgetEnumerable result)
    {
        _hitBoxGadgetSpacialHashGrid.GetAllItemsNearPosition(levelPosition, out result);
    }

    public void GetAllItemsNearRegion(RectangularRegion levelRegion, out GadgetEnumerable result)
    {
        _hitBoxGadgetSpacialHashGrid.GetAllItemsNearRegion(levelRegion, out result);
    }

    public void UpdateGadgetPosition(HitBoxGadget gadget)
    {
        _hitBoxGadgetSpacialHashGrid.UpdateItemPosition(gadget);
    }

    public void ResetGadgetPositions()
    {
        _hitBoxGadgetSpacialHashGrid.Clear();

        foreach (GadgetBase gadget in _allGadgets)
        {
            if (gadget is HitBoxGadget hitBoxGadget)
            {
                _hitBoxGadgetSpacialHashGrid.AddItem(hitBoxGadget);
            }
        }
    }

    int IPerfectHasher<GadgetBase>.NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase IPerfectHasher<GadgetBase>.UnHash(int index) => _allGadgets[index];

    void IBitBufferCreator<RawBitBuffer, GadgetBase>.CreateBitBuffer(out RawBitBuffer buffer) =>
        buffer = GetNextRawBitBuffer();

    int IPerfectHasher<HitBoxGadget>.NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget IPerfectHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];

    void IBitBufferCreator<RawBitBuffer, HitBoxGadget>.CreateBitBuffer(out RawBitBuffer buffer) =>
        buffer = GetNextRawBitBuffer();

    int IPerfectHasher<GadgetTrigger>.NumberOfItems => _allTriggers.Length;
    int IPerfectHasher<GadgetTrigger>.Hash(GadgetTrigger item) => item.Id;
    GadgetTrigger IPerfectHasher<GadgetTrigger>.UnHash(int index) => _allTriggers[index];

    void IBitBufferCreator<ArrayBitBuffer, GadgetTrigger>.CreateBitBuffer(out ArrayBitBuffer buffer) =>
        buffer = new(_allTriggers.Length);

    int IPerfectHasher<GadgetBehaviour>.NumberOfItems => _allBehaviours.Length;
    int IPerfectHasher<GadgetBehaviour>.Hash(GadgetBehaviour item) => item.Id;
    GadgetBehaviour IPerfectHasher<GadgetBehaviour>.UnHash(int index) => _allBehaviours[index];

    private unsafe RawBitBuffer GetNextRawBitBuffer()
    {
        if (_bitArrayBufferUsageCount == 0)
            throw new InvalidOperationException("Insufficient space for bit buffers!");
        _bitArrayBufferUsageCount--;
        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_allGadgets.Length);

        uint* pointer = (uint*)_gadgetByteBuffer.Handle + (bitBufferLength * _bitArrayBufferUsageCount);
        return new RawBitBuffer(pointer, bitBufferLength);
    }

    public void Dispose()
    {
        new Span<GadgetBase>(_allGadgets).Clear();
        _gadgetByteBuffer.Dispose();
        _hitBoxGadgetSpacialHashGrid.Dispose();
    }
}