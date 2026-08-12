using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class PrePlacedLemmingDataSectionReader : LevelDataSectionReader
{
    public PrePlacedLemmingDataSectionReader()
        : base(LevelFileSectionIdentifier.PrePlacedLemmingDataSection, false)
    {
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.PrePlacedLemmingData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var lemmingData = ReadLemmingData(reader);

            levelData.PrePlacedLemmingData.Add(lemmingData);
        }
    }

    private static LemmingInstanceData ReadLemmingData(RawLevelFileDataReader reader)
    {
        int positionData = reader.Read32BitSignedInteger();
        var position = ReadWriteHelpers.DecodePoint(positionData);

        uint state = reader.Read32BitUnsignedInteger();

        int dhtByte = reader.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int tribeId = reader.Read8BitUnsignedInteger();
        uint rawLemmingActionType = reader.Read8BitUnsignedInteger();
        var lemmingActionType = LemmingActionConstants.GetEnumValue(rawLemmingActionType);

        return new LemmingInstanceData
        {
            Position = position,
            State = state,

            DihedralTransformation = dht,

            TribeId = tribeId,
            InitialLemmingActionType = lemmingActionType
        };
    }
}
