using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class HatchGroupBuilder
{
    private readonly LevelData _levelData;

    public RawArray HatchGroupDataBuffer { get; }

    public HatchGroupBuilder(LevelData levelData, SafeBufferAllocator safeBufferAllocator)
    {
        _levelData = levelData;

        var numberOfHatchGroups = _levelData.AllHatchGroupData.Count;
        HatchGroupDataBuffer = safeBufferAllocator.AllocateRawArray(numberOfHatchGroups * Level.Gadgets.HatchGadgets.HatchGroupData.HatchGroupDataSize);
    }

    public HatchGroup[] BuildHatchGroups()
    {
        var allHatchGroupData = CollectionsMarshal.AsSpan(_levelData.AllHatchGroupData);

        var result = new HatchGroup[allHatchGroupData.Length];
        var handle = HatchGroupDataBuffer.Handle;

        for (var i = 0; i < allHatchGroupData.Length; i++)
        {
            var prototype = allHatchGroupData[i];
            var hatchGroup = new HatchGroup(
                handle,
                i,
                prototype.MinSpawnInterval,
                prototype.MaxSpawnInterval,
                prototype.InitialSpawnInterval);

            result[i] = hatchGroup;
            handle += Level.Gadgets.HatchGadgets.HatchGroupData.HatchGroupDataSize;
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
