using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class HatchGroupBuilder
{
    public static HatchGroup[] BuildHatchGroups(LevelData levelData)
    {
        var allHatchGroupData = CollectionsMarshal.AsSpan(levelData.AllHatchGroupData);

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

    public static void SetHatchesForHatchGroup(
        HatchGroup hatchGroup,
        ReadOnlySpan<GadgetBase> gadgetSpan)
    {
        var hatches = new List<HatchGadget>();

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
