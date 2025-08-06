using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager :
    IPerfectHasher<GadgetBase>,
    IPerfectHasher<HitBoxGadget>,
    IItemManager<GadgetBase>,
    IBitBufferCreator<ArrayBitBuffer>,
    ISnapshotDataConvertible<int>,
    IInitialisable,
    IDisposable
{
    private readonly GadgetBase[] _allGadgets;
    private readonly GadgetBehaviour[] _allGadgetBehaviours;
    private readonly uint[] _bitBuffer;
    private readonly HitBoxGadgetSpacialHashGrid _hitBoxGadgetSpacialHashGrid;
    private readonly GadgetSet _fastForwardGadgets;

    private int _bitArrayBufferUsageCount;

    public ReadOnlySpan<GadgetBase> AllItems => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        GadgetBehaviour[] allGadgetBehaviours,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        this.AssertUniqueIds(new ReadOnlySpan<GadgetBase>(allGadgets));
        Array.Sort(_allGadgets, this);

        _allGadgetBehaviours = allGadgetBehaviours;

        // 1 spacial hash grid + 1 gadget set
        const int ExpectedNumberOfGadgetBitSets = 2;
        _bitArrayBufferUsageCount = ExpectedNumberOfGadgetBitSets;

        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_allGadgets.Length);
        _bitBuffer = new uint[bitBufferLength * ExpectedNumberOfGadgetBitSets];

        _hitBoxGadgetSpacialHashGrid = new HitBoxGadgetSpacialHashGrid(
            this,
            EngineConstants.GadgetPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _fastForwardGadgets = new GadgetSet(this);
    }

    public int ScratchSpaceSize => _hitBoxGadgetSpacialHashGrid.ScratchSpaceSize;

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

    public void Tick(bool isMajorTick)
    {
        foreach (var gadgetBehaviour in _allGadgetBehaviours)
        {
            gadgetBehaviour.Reset();
        }

        if (isMajorTick)
        {
            var gadgetSpan = new ReadOnlySpan<GadgetBase>(_allGadgets);
            foreach (var gadget in gadgetSpan)
            {
                gadget.Tick();
            }
        }
        else
        {
            foreach (var gadget in _fastForwardGadgets)
            {
                gadget.Tick();
            }
        }
    }

    public void GetAllGadgetsNearPosition(
        Span<uint> scratchSpaceSpan,
        Point levelPosition,
        out GadgetEnumerable result)
    {
        _hitBoxGadgetSpacialHashGrid.GetAllItemsNearPosition(
            scratchSpaceSpan,
            levelPosition,
            out result);
    }

    public void GetAllItemsNearRegion(
        Span<uint> scratchSpace,
        RectangularRegion levelRegion,
        out GadgetEnumerable result)
    {
        _hitBoxGadgetSpacialHashGrid.GetAllItemsNearRegion(
            scratchSpace,
            levelRegion,
            out result);
    }

    public void UpdateGadgetPosition(HitBoxGadget gadget)
    {
        _hitBoxGadgetSpacialHashGrid.UpdateItemPosition(gadget);
    }

    public void WriteToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
        ResetGadgetPositions();
    }

    private void ResetGadgetPositions()
    {
        _hitBoxGadgetSpacialHashGrid.Clear();

        var gadgets = AllItems;

        for (var i = 0; i < gadgets.Length; i++)
        {
            if (gadgets[i] is HitBoxGadget hitBoxGadget)
            {
                _hitBoxGadgetSpacialHashGrid.AddItem(hitBoxGadget);
            }
        }
    }

    public int NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase IPerfectHasher<GadgetBase>.UnHash(int index) => _allGadgets[index];
    int IPerfectHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget IPerfectHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];
    void IBitBufferCreator<ArrayBitBuffer>.CreateBitBuffer(int numberOfItems, out ArrayBitBuffer buffer)
    {
        if (_bitArrayBufferUsageCount == 0)
            throw new InvalidOperationException("Insufficient space for bit buffers!");
        _bitArrayBufferUsageCount--;
        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_allGadgets.Length);
        buffer = new(_bitBuffer, bitBufferLength * _bitArrayBufferUsageCount, bitBufferLength);
    }

    public void Dispose()
    {
        Array.Clear(_allGadgets);
        _hitBoxGadgetSpacialHashGrid.Dispose();
    }
}
