using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : ISimpleHasher<GadgetBase>
{
    private readonly GadgetBase[] _allGadgets;
    private readonly PositionHelper<GadgetBase> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        _allGadgets.ValidateUniqueIds();
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new PositionHelper<GadgetBase>(
            this,
            ChunkSizeType.ChunkSize64,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    public void Initialise()
    {
        foreach (var gadget in _allGadgets)
        {
            if (gadget.CaresAboutLemmingInteraction)
            {
                _gadgetPositionHelper.UpdateItemPosition(gadget, true);
            }
        }
    }

    [Pure]
    public LargeSimpleSet<GadgetBase>.Enumerator GetAllGadgetsForPosition(LevelPosition levelPosition)
    {
        return _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);
    }

    [Pure]
    public bool HasGadgetOfTypeAtPosition(LevelPosition levelPosition, GadgetType gadgetType)
    {
        var gadgetEnumerator = GetAllGadgetsForPosition(levelPosition);

        while (gadgetEnumerator.MoveNext())
        {
            var gadget = gadgetEnumerator.Current;

            if (gadget.Type != gadgetType)
                continue;
            if (gadget.MatchesPosition(levelPosition))
                return true;
        }

        return false;
    }

    public void UpdateGadgetPosition(int gadgetId)
    {
        var gadget = _allGadgets[gadgetId];
        UpdateGadgetPosition(gadget);
    }

    public void UpdateGadgetPosition(GadgetBase gadget)
    {
        if (gadget.CaresAboutLemmingInteraction)
        {
            _gadgetPositionHelper.UpdateItemPosition(gadget, false);
        }
    }

    int ISimpleHasher<GadgetBase>.NumberOfItems => _allGadgets.Length;
    int ISimpleHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase ISimpleHasher<GadgetBase>.Unhash(int index) => _allGadgets[index];
}