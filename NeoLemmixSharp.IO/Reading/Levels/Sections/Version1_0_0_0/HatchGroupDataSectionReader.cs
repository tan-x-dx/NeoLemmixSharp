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

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.AllHatchGroupData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var hatchGroupData = ReadHatchGroupData(reader);

            levelData.AllHatchGroupData.Add(hatchGroupData);
        }
    }

    private static HatchGroupData ReadHatchGroupData(RawLevelFileDataReader reader)
    {
        int hatchGroupId = reader.Read8BitUnsignedInteger();
        int minSpawnInterval = reader.Read8BitUnsignedInteger();
        int maxSpawnInterval = reader.Read8BitUnsignedInteger();
        int initialSpawnInterval = reader.Read8BitUnsignedInteger();

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