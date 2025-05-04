using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class HatchGroupDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.HatchGroupDataSection;
    public override bool IsNecessary => false;

    public HatchGroupDataSectionReader(
        Version version)
    {
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllHatchGroupData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var hatchGroupData = ReadHatchGroupData(rawFileData);

            levelData.AllHatchGroupData.Add(hatchGroupData);
        }
    }

    private static HatchGroupData ReadHatchGroupData(RawLevelFileDataReader rawFileData)
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