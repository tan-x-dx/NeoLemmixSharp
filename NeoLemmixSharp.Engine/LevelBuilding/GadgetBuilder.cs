using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

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

    public readonly HatchGroup[] BuildHatchGroups()
    {
        var allHatchGroupData = CollectionsMarshal.AsSpan(_levelData.AllHatchGroupData);

        var result = new HatchGroup[allHatchGroupData.Length];

        for (var i = 0; i < allHatchGroupData.Length; i++)
        {
            var prototype = allHatchGroupData[i];
            var hatchGroup = new HatchGroup(
                i,
                prototype.MinSpawnInterval,
                prototype.MaxSpawnInterval,
                prototype.InitialSpawnInterval);

            result[i] = hatchGroup;
        }

        return result;
    }

    public GadgetBase[] BuildLevelGadgets(
        LemmingManager lemmingHasher,
        TribeManager tribeManager)
    {
        var allGadgetData = CollectionsMarshal.AsSpan(_levelData.AllGadgetData);

        foreach (var prototype in allGadgetData)
        {
            var gadgetArchetypeData = _gadgetArchetypeDataLookup[prototype.GetStylePiecePair()];

            var gadget = Gadgets.GadgetBuilder.BuildGadget(gadgetArchetypeData, prototype, lemmingHasher, tribeManager);
            _gadgetList.Add(gadget);
        }

        return _gadgetList.GetArray();
    }

    public readonly void SetHatchesForHatchGroup(HatchGroup hatchGroup)
    {
        var hatches = new List<HatchGadget>();

        var gadgetSpan = _gadgetList.AsSpan();
        foreach (var gadget in gadgetSpan)
        {
            if (gadget is HatchGadget hatch &&
                hatch.HatchSpawnData.HatchGroupId == hatchGroup.Id)
            {
                hatches.Add(hatch);
            }
        }

        hatchGroup.SetHatches(hatches.ToArray());
    }
}