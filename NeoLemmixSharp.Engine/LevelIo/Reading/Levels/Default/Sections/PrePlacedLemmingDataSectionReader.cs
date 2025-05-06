using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;

public sealed class PrePlacedLemmingDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.PrePlacedLemmingDataSection;
    public override bool IsNecessary => false;

    public PrePlacedLemmingDataSectionReader(
        Version version)
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
        int initialActionId = rawFileData.Read8BitUnsignedInteger();

        var initialLemmingAction = LemmingAction.GetActionOrDefault(initialActionId);
        FileReadingException.ReaderAssert(initialLemmingAction != NoneAction.Instance, "Invalid initial action for lemming!");

        return new LemmingData
        {
            Position = new Point(x, y),
            State = state,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            TeamId = teamId,
            InitialLemmingAction = initialLemmingAction
        };
    }
}