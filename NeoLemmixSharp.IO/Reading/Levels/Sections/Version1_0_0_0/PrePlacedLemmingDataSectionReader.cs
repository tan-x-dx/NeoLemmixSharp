using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

public sealed class PrePlacedLemmingDataSectionReader : LevelDataSectionReader
{
    public PrePlacedLemmingDataSectionReader()
        : base(LevelFileSectionIdentifier.PrePlacedLemmingDataSection, false)
    {
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.PrePlacedLemmingData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var lemmingData = ReadLemmingData(rawFileData);

            levelData.PrePlacedLemmingData.Add(lemmingData);
        }
    }

    private static LemmingData ReadLemmingData(RawLevelFileDataReader rawFileData)
    {
        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        x -= LevelReadWriteHelpers.PositionOffset;
        y -= LevelReadWriteHelpers.PositionOffset;

        uint state = rawFileData.Read32BitUnsignedInteger();

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int teamId = rawFileData.Read8BitUnsignedInteger();
        int initialLemmingActionId = rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(EngineConstants.IsValidLemmingActionId(initialLemmingActionId), "Invalid initial action for lemming!");

        return new LemmingData
        {
            Position = new Point(x, y),
            State = state,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            TeamId = teamId,
            InitialLemmingActionId = initialLemmingActionId
        };
    }
}