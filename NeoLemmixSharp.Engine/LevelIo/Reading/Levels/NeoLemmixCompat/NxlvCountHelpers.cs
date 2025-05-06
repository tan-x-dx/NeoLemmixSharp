using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat.Readers.GadgetReaders;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat;

public static class NxlvCountHelpers
{
    private const int MaxStackallocSize = 64;

    public static void CalculateHatchCounts(
        LevelData levelData,
        LevelDataReader levelDataReader,
        GadgetReader gadgetReader)
    {
    }
    /*var gadgetDataSpan = CollectionsMarshal.AsSpan(gadgetReader.AllGadgetData);
    var hatchGadgets = new List<NeoLemmixGadgetData>();

    foreach (var gadgetData in gadgetDataSpan)
    {
        if (GadgetDataIsHatch(gadgetReader.GadgetArchetypes, gadgetData))
        {
            hatchGadgets.Add(gadgetData);
        }
    }

    var maxLemmingCountFromHatches = 0;
    var hasInfiniteHatchCount = false;
    var spanLength = hatchGadgets.Count;
    Span<HatchCountData> countSpan = spanLength > MaxStackallocSize
        ? new HatchCountData[spanLength]
        : stackalloc HatchCountData[spanLength];

    var i = 0;
    while (i < countSpan.Length)
    {
        var correspondingHatchGadget = hatchGadgets[i];
        countSpan[i].RunningCount = 0;
        countSpan[i].MaxCount = correspondingHatchGadget.LemmingCount ?? -1;
        hasInfiniteHatchCount |= !correspondingHatchGadget.LemmingCount.HasValue;
        maxLemmingCountFromHatches += correspondingHatchGadget.LemmingCount ?? 0;

        i++;
    }

    var numberOfLemmingsFromHatches = CalculateTotalHatchLemmingCounts(
        levelData,
        levelDataReader,
        hasInfiniteHatchCount ? null : maxLemmingCountFromHatches);

    var numberOfLemmingsAssignedToHatches = 0;
    i = 0;
    while (numberOfLemmingsAssignedToHatches < numberOfLemmingsFromHatches)
    {
        ref var runningCount = ref countSpan[i].RunningCount;
        var maxCount = countSpan[i].MaxCount;

        if (maxCount == -1 || runningCount < maxCount)
        {
            runningCount++;
            numberOfLemmingsAssignedToHatches++;
        }

        i++;
        if (i == countSpan.Length)
        {
            i = 0;
        }
    }

    i = 0;
    while (i < countSpan.Length)
    {
        var hatchCount = countSpan[i].RunningCount;
        var correspondingHatchGadget = hatchGadgets[i];
        correspondingHatchGadget.LemmingCount = hatchCount;
        i++;
    }
}

private static bool GadgetDataIsHatch(
    Dictionary<string, NeoLemmixGadgetArchetypeData> gadgetArchetypes,
    NeoLemmixGadgetData gadgetData)
{
    var gadgetArchetypeId = gadgetData.GadgetArchetypeId;

    var gadgetArchetypeData = gadgetArchetypes
        .Values
        .First(a => a.GadgetArchetypeId == gadgetArchetypeId);

    return gadgetArchetypeData.Behaviour == NeoLemmixGadgetBehaviour.Entrance;
}

private static int CalculateTotalHatchLemmingCounts(
    LevelData levelData,
    LevelDataReader levelDataReader,
    int? maxLemmingCountFromHatches)
{
    var hatchLemmingData = levelData.HatchLemmingData;
    var hatchLemmingCount = levelDataReader.NumberOfLemmings - levelData.PrePlacedLemmingData.Count;

    var totalNumberOfHatchLemmings = maxLemmingCountFromHatches.HasValue
        ? Math.Min(hatchLemmingCount, maxLemmingCountFromHatches.Value)
        : hatchLemmingCount;

    hatchLemmingData.Capacity = totalNumberOfHatchLemmings;

    while (hatchLemmingData.Count < hatchLemmingData.Capacity)
    {
        hatchLemmingData.Add(new LemmingData());
    }

    return totalNumberOfHatchLemmings;
}*/

    private struct HatchCountData
    {
        public int RunningCount;
        public int MaxCount;
    }
}
