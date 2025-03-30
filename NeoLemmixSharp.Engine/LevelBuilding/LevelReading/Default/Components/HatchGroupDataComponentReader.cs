using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class HatchGroupDataComponentReader : ILevelDataReader
{
    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.HatchGroupDataSectionIdentifier;

    public HatchGroupDataComponentReader(
        Version version)
    {
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllHatchGroupData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            int hatchGroupId = rawFileData.Read8BitUnsignedInteger();
            int minSpawnInterval = rawFileData.Read8BitUnsignedInteger();
            int maxSpawnInterval = rawFileData.Read8BitUnsignedInteger();
            int initialSpawnInterval = rawFileData.Read8BitUnsignedInteger();

            LevelReadWriteHelpers.ReaderAssert(minSpawnInterval >= EngineConstants.MinAllowedSpawnInterval, "Invalid MinSpawnInterval");
            LevelReadWriteHelpers.ReaderAssert(maxSpawnInterval <= EngineConstants.MaxAllowedSpawnInterval, "Invalid MaxSpawnInterval");
            LevelReadWriteHelpers.ReaderAssert(minSpawnInterval <= initialSpawnInterval && initialSpawnInterval <= maxSpawnInterval, "Invalid Spawn Interval Limits");

            levelData.AllHatchGroupData.Add(new HatchGroupData
            {
                HatchGroupId = hatchGroupId,
                MinSpawnInterval = minSpawnInterval,
                MaxSpawnInterval = maxSpawnInterval,
                InitialSpawnInterval = initialSpawnInterval
            });
        }
    }
}