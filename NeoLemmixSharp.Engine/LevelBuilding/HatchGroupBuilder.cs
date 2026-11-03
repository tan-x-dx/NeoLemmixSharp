using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class HatchGroupBuilder
{
    private readonly LevelData _levelData;

    public RawArray HatchGroupDataBuffer { get; }

    public HatchGroupBuilder(LevelData levelData, SafeBufferAllocator safeBufferAllocator)
    {
        _levelData = levelData;

        var numberOfHatchGroups = _levelData.AllHatchGroupData.Count;
        HatchGroupDataBuffer = safeBufferAllocator.AllocateRawArray<Level.Gadgets.HatchGadgets.HatchGroupData>(numberOfHatchGroups);
    }

    public HatchGroup[] BuildHatchGroups()
    {
        var result = new HatchGroup[_levelData.AllHatchGroupData.Count];
        var dataHandle = new DataHandle(HatchGroupDataBuffer);

        for (var i = 0; i < _levelData.AllHatchGroupData.Count; i++)
        {
            var prototype = _levelData.AllHatchGroupData[i];
            var hatchGroup = new HatchGroup(
                dataHandle,
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
