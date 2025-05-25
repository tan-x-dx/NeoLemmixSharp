using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public ref struct GadgetBuilder
{
    private readonly LevelData _levelData;
    private readonly Dictionary<StylePiecePair, GadgetArchetypeData> _gadgetArchetypeDataLookup;
    private ArrayListWrapper<GadgetBase> _gadgetList;

    public GadgetBuilder(LevelData levelData)
    {
        _levelData = levelData;
        _gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(levelData);
        _gadgetList = new ArrayListWrapper<GadgetBase>(levelData.AllGadgetData.Count);
    }

    public GadgetBase[] BuildLevelGadgets(
        LemmingManager lemmingHasher,
        TribeManager tribeManager)
    {
        var allGadgetData = CollectionsMarshal.AsSpan(_levelData.AllGadgetData);

        foreach (var prototype in allGadgetData)
        {
            var gadgetArchetypeData = _gadgetArchetypeDataLookup[prototype.GetStylePiecePair()];

            var gadget = BuildGadget(gadgetArchetypeData, prototype, lemmingHasher, tribeManager);
            _gadgetList.Add(gadget);
        }

        return _gadgetList.GetArray();
    }
    public static GadgetBase BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData prototype,
        LemmingManager lemmingHasher,
        TribeManager tribeManager)
    {
        switch (gadgetArchetypeData.GadgetType)
        {
            case GadgetType.HitBoxGadget:
                break;
            case GadgetType.HatchGadget:
                break;
            case GadgetType.GadgetMover:
                break;
            case GadgetType.GadgetResizer:
                break;
            case GadgetType.GadgetStateChanger:
                break;
            case GadgetType.AndGate:
                break;
            case GadgetType.OrGate:
                break;
            case GadgetType.NotGate:
                break;
            case GadgetType.XorGate:
                break;

            default:
                break;
        }












        throw new NotImplementedException();
    }
}