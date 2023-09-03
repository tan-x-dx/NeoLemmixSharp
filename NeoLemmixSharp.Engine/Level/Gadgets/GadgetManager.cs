using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager
{
    private readonly GadgetBase[] _allGadgets;
    private readonly ChunkManager<GadgetBase> _gadgetChunkManager;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        _gadgetChunkManager = new ChunkManager<GadgetBase>(allGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        foreach (var gadget in allGadgets)
        {
            if (gadget.CaresAboutLemmingInteraction)
            {
                UpdateGadgetPosition(gadget);
            }
        }
    }

    [Pure]
    public LargeBitArray.Enumerator GetAllGadgetIdsForPosition(LevelPosition levelPosition)
    {
        return _gadgetChunkManager.GetAllItemIdsForPosition(levelPosition);
    }

    [Pure]
    public bool HasGadgetOfTypeAtPosition(LevelPosition levelPosition, GadgetType gadgetType)
    {
        var allGadgets = AllGadgets;
        var idEnumerator = GetAllGadgetIdsForPosition(levelPosition);

        while (idEnumerator.MoveNext())
        {
            var gadgetId = idEnumerator.Current;
            var gadget = allGadgets[gadgetId];

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
            _gadgetChunkManager.UpdateItemPosition(gadget);
        }
    }
}