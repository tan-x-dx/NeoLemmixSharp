using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager :
    IBitBufferCreator<RawBitBuffer, HitBoxGadget>,
    IBitBufferCreator<RawBitBuffer, GadgetBase>,
    IBitBufferCreator<ArrayBitBuffer, GadgetTrigger>,
    IPerfectHasher<GadgetBehaviour>,
    IInitialisable,
    IDisposable
{
    private readonly GadgetBase[] _allGadgets;
    private readonly GadgetTrigger[] _allTriggers;
    private readonly GadgetBehaviour[] _allBehaviours;
    private readonly BitArraySet<GadgetManager, ArrayBitBuffer, GadgetTrigger> _indeterminateTriggers;
    private readonly List<GadgetLemmingInteraction> _gadgetLemmingInteractions = new(256);

    private readonly HitBoxGadgetSpacialHashGrid _hitBoxGadgetSpacialHashGrid;
    private readonly GadgetSet _fastForwardGadgets;
    private readonly GadgetSet _gadgetsToReEvaluate;

    private const int RequiredNumberOfGadgetBitSets =
        1 + // spacial hash grid
        2; // gadget sets

    private readonly RawArray _gadgetByteBuffer;
    private int _bitArrayBufferUsageCount = RequiredNumberOfGadgetBitSets;

    private readonly LemmingTrackerManager _lemmingTrackerManager;

    private bool _isDisposed;

    public ReadOnlySpan<GadgetBase> AllItems => new(_allGadgets);
    public ReadOnlySpan<GadgetBehaviour> AllBehaviours => new(_allBehaviours);
    public GadgetBase GetGadget(int gadgetId) => _allGadgets[gadgetId];

    public GadgetManager(
        GadgetBase[] allGadgets,
        GadgetTrigger[] allTriggers,
        GadgetBehaviour[] allBehaviours,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour,
        LemmingTrackerManager lemmingTrackerManager)
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

        _lemmingTrackerManager = lemmingTrackerManager;
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
        _gadgetLemmingInteractions.Clear();
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
        _lemmingTrackerManager.Tick();

        if (isMajorTick)
        {
            TickAllGadgets();
        }
        else
        {
            TickFastForwardGadgets();
        }

        foreach (var gadgetLemmingInteraction in _gadgetLemmingInteractions)
        {
            var gadgetBehaviour = _allBehaviours[gadgetLemmingInteraction.LemmingBehaviourId];

            if (gadgetBehaviour is LemmingBehaviour lemmingBehaviour)
            {
                lemmingBehaviour.PerformBehaviour(gadgetLemmingInteraction.LemmingId);
            }
            else
            {
                gadgetBehaviour.PerformBehaviour();
            }
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

    public void RegisterCauseAndEffectData(GadgetBehaviour gadgetBehaviour, Lemming lemming)
    {
        _gadgetLemmingInteractions.Add(new GadgetLemmingInteraction(gadgetBehaviour.Id, lemming.Id));
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
            _indeterminateTriggers.Remove(trigger);
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

    public void OnSnapshotApplied()
    {
        ResetGadgetPositions();
    }

    private void ResetGadgetPositions()
    {
        _hitBoxGadgetSpacialHashGrid.Clear();

        foreach (var gadget in _allGadgets)
        {
            if (gadget is HitBoxGadget hitBoxGadget)
            {
                _hitBoxGadgetSpacialHashGrid.AddItem(hitBoxGadget);
            }
        }
    }

    int IPerfectHasher<GadgetBase>.NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase IPerfectHasher<GadgetBase>.UnHash(int index) => _allGadgets.At(index);

    void IBitBufferCreator<RawBitBuffer, GadgetBase>.CreateBitBuffer(out RawBitBuffer buffer) =>
        buffer = GetNextRawBitBuffer();

    int IPerfectHasher<HitBoxGadget>.NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget IPerfectHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets.At(index);

    void IBitBufferCreator<RawBitBuffer, HitBoxGadget>.CreateBitBuffer(out RawBitBuffer buffer) =>
        buffer = GetNextRawBitBuffer();

    int IPerfectHasher<GadgetTrigger>.NumberOfItems => _allTriggers.Length;
    int IPerfectHasher<GadgetTrigger>.Hash(GadgetTrigger item) => item.Id;
    GadgetTrigger IPerfectHasher<GadgetTrigger>.UnHash(int index) => _allTriggers.At(index);

    void IBitBufferCreator<ArrayBitBuffer, GadgetTrigger>.CreateBitBuffer(out ArrayBitBuffer buffer) =>
        buffer = new(_allTriggers.Length);

    int IPerfectHasher<GadgetBehaviour>.NumberOfItems => _allBehaviours.Length;
    int IPerfectHasher<GadgetBehaviour>.Hash(GadgetBehaviour item) => item.Id;
    GadgetBehaviour IPerfectHasher<GadgetBehaviour>.UnHash(int index) => _allBehaviours.At(index);

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
        if (!_isDisposed)
        {
            _isDisposed = true;

            new Span<GadgetBase>(_allGadgets).Clear();
            _gadgetByteBuffer.Dispose();
            _hitBoxGadgetSpacialHashGrid.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
