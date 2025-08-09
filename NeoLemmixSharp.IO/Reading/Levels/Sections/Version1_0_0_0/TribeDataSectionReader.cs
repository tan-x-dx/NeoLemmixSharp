using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class TribeDataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public TribeDataSectionReader(FileReaderStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TribeDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(numberOfItemsInSection <= EngineConstants.MaxNumberOfTribes, "Too many tribes specified!");

        levelData.TribeIdentifiers.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var tribeIdentifierData = ReadTribeIdentifierData(reader);

            levelData.TribeIdentifiers.Add(tribeIdentifierData);
        }
    }

    private TribeStyleIdentifier ReadTribeIdentifierData(RawLevelFileDataReader reader)
    {
        int styleId = reader.Read16BitUnsignedInteger();
        var styleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]);
        int themeTribeId = reader.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(themeTribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id!");

        return new TribeStyleIdentifier(styleIdentifier, themeTribeId);
    }
}
