using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class HatchGroupDataSectionReader : LevelDataSectionReader
{
    public HatchGroupDataSectionReader()
        : base(LevelFileSectionIdentifier.HatchGroupDataSection, false)
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

        FileReadingException.ReaderAssert(minSpawnInterval >= EngineConstants.MinAllowedSpawnInterval, "Invalid MinSpawnInterval");
        FileReadingException.ReaderAssert(maxSpawnInterval <= EngineConstants.MaxAllowedSpawnInterval, "Invalid MaxSpawnInterval");
        FileReadingException.ReaderAssert(minSpawnInterval <= initialSpawnInterval && initialSpawnInterval <= maxSpawnInterval, "Invalid Spawn Interval Limits");

        return new HatchGroupData
        {
            HatchGroupId = hatchGroupId,
            MinSpawnInterval = minSpawnInterval,
            MaxSpawnInterval = maxSpawnInterval,
            InitialSpawnInterval = initialSpawnInterval
        };
    }

}