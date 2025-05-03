using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Components;

public sealed class HatchGroupDataComponentReader : LevelDataComponentReader
{
    public HatchGroupDataComponentReader(
        Version version)
        : base(LevelFileSectionIdentifier.HatchGroupDataSection)
    {
    }

    public override void ReadSection(RawLevelFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllHatchGroupData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var hatchGroupData = ReadHatchGroupData(rawFileData);

            levelData.AllHatchGroupData.Add(hatchGroupData);
        }
    }

    private static HatchGroupData ReadHatchGroupData(RawLevelFileData rawFileData)
    {
        int hatchGroupId = rawFileData.Read8BitUnsignedInteger();
        int minSpawnInterval = rawFileData.Read8BitUnsignedInteger();
        int maxSpawnInterval = rawFileData.Read8BitUnsignedInteger();
        int initialSpawnInterval = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(minSpawnInterval >= EngineConstants.MinAllowedSpawnInterval, "Invalid MinSpawnInterval");
        LevelReadingException.ReaderAssert(maxSpawnInterval <= EngineConstants.MaxAllowedSpawnInterval, "Invalid MaxSpawnInterval");
        LevelReadingException.ReaderAssert(minSpawnInterval <= initialSpawnInterval && initialSpawnInterval <= maxSpawnInterval, "Invalid Spawn Interval Limits");

        return new HatchGroupData
        {
            HatchGroupId = hatchGroupId,
            MinSpawnInterval = minSpawnInterval,
            MaxSpawnInterval = maxSpawnInterval,
            InitialSpawnInterval = initialSpawnInterval
        };
    }
}