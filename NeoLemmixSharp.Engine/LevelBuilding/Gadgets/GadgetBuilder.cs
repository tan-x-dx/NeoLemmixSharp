using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;

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
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        foreach (var prototype in _levelData.AllGadgetData)
        {
            var gadgetArchetypeData = _gadgetArchetypeDataLookup[prototype.GetStylePiecePair()];

            var gadget = BuildGadget(gadgetArchetypeData, prototype, lemmingManager, tribeManager);
            _gadgetList.Add(gadget);
        }

        return _gadgetList.GetArray();
    }

    private static GadgetBase BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData prototype,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        return gadgetArchetypeData.GadgetType switch
        {
            GadgetType.HitBoxGadget => HitBoxGadgetArchetypeBuilder.BuildGadget(gadgetArchetypeData, prototype, lemmingManager, tribeManager),
            GadgetType.HatchGadget => HatchGadgetArchetypeBuilder.BuildGadget(gadgetArchetypeData, prototype, lemmingManager, tribeManager),

            GadgetType.GadgetMover => null,
            GadgetType.GadgetResizer => null,
            GadgetType.GadgetStateChanger => null,

            GadgetType.AndGate => LogicGateArchetypeBuilder.BuildAndGateGadget(gadgetArchetypeData, prototype),
            GadgetType.OrGate => LogicGateArchetypeBuilder.BuildOrGateGadget(gadgetArchetypeData, prototype),
            GadgetType.NotGate => LogicGateArchetypeBuilder.BuildNotGateGadget(gadgetArchetypeData, prototype),
            GadgetType.XorGate => LogicGateArchetypeBuilder.BuildXorGateGadget(gadgetArchetypeData, prototype),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetType, GadgetBase>(gadgetArchetypeData.GadgetType)
        };
    }
}
