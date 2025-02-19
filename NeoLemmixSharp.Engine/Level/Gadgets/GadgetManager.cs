using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
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
    private readonly GadgetSpacialHashGrid _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllItems => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<GadgetBase>(allGadgets));
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new GadgetSpacialHashGrid(
            this,
            EngineConstants.GadgetPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    public int ScratchSpaceSize => _gadgetPositionHelper.ScratchSpaceSize;

    public void Initialise()
    {
        foreach (var gadget in _allGadgets)
        {
            if (gadget is HitBoxGadget hitBoxGadget)
            {
                _gadgetPositionHelper.AddItem(hitBoxGadget);
            }
        }
    }

    public void Tick(bool isMajorTick)
    {
        foreach (var gadget in _allGadgets)
        {
            if (isMajorTick)
            {
                gadget.Tick();
            }
        }
    }

    public void GetAllGadgetsNearPosition(
        Span<uint> scratchSpaceSpan,
        LevelPosition levelPosition,
        out GadgetEnumerable result)
    {
        _gadgetPositionHelper.GetAllItemsNearPosition(
            scratchSpaceSpan,
            levelPosition,
            out result);
    }

    public void GetAllItemsNearRegion(
        Span<uint> scratchSpace,
        LevelRegion levelRegion,
        out GadgetEnumerable result)
    {
        _gadgetPositionHelper.GetAllItemsNearRegion(
            scratchSpace,
            levelRegion,
            out result);
    }

    public void UpdateGadgetPosition(HitBoxGadget gadget)
    {
        _gadgetPositionHelper.UpdateItemPosition(gadget);
    }

    public int NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase IPerfectHasher<GadgetBase>.UnHash(int index) => _allGadgets[index];
    int IPerfectHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget IPerfectHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];
    void IBitBufferCreator<ArrayBitBuffer>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(BitArrayHelpers.CreateBitArray(NumberOfItems, false));

    public void Dispose()
    {
        Array.Clear(_allGadgets);
        _gadgetPositionHelper.Clear();
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
        _gadgetPositionHelper.Clear();

        var gadgets = AllItems;

        for (var i = 0; i < gadgets.Length; i++)
        {
            if (gadgets[i] is HitBoxGadget hitBoxGadget)
            {
                _gadgetPositionHelper.AddItem(hitBoxGadget);
            }
        }
    }
}