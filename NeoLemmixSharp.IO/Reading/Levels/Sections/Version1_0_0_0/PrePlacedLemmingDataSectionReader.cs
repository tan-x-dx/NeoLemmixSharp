using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class PrePlacedLemmingDataSectionReader : LevelDataSectionReader
{
    public PrePlacedLemmingDataSectionReader()
        : base(LevelFileSectionIdentifier.PrePlacedLemmingDataSection, false)
    {
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.PrePlacedLemmingData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var lemmingData = ReadLemmingData(rawFileData);

            levelData.PrePlacedLemmingData.Add(lemmingData);
        }
    }

    private static LemmingData ReadLemmingData(RawLevelFileDataReader rawFileData)
    {
        int positionData = rawFileData.Read32BitSignedInteger();
        var position = ReadWriteHelpers.DecodePoint(positionData);

        uint state = rawFileData.Read32BitUnsignedInteger();

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int tribeId = rawFileData.Read8BitUnsignedInteger();
        int initialLemmingActionId = rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(LemmingActionConstants.IsValidLemmingActionId(initialLemmingActionId), "Invalid initial action for lemming!");

        return new LemmingData
        {
            Position = position,
            State = state,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            TribeId = tribeId,
            InitialLemmingActionId = initialLemmingActionId
        };
    }
}