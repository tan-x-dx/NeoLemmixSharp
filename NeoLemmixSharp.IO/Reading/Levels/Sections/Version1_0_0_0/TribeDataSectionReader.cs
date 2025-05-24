using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class TribeDataSectionReader : LevelDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;

    public TribeDataSectionReader(StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TribeDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(numberOfItemsInSection <= EngineConstants.MaxNumberOfTribes, "Too many tribes specified!");

        levelData.TribeIdentifiers.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var tribeIdentifierData = ReadTribeIdentifierData(rawFileData);

            levelData.TribeIdentifiers.Add(tribeIdentifierData);
        }
    }

    private TribeIdentifier ReadTribeIdentifierData(RawLevelFileDataReader rawFileData)
    {
        int styleId = rawFileData.Read16BitUnsignedInteger();
        var styleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]);
        int themeTribeId = rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(themeTribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id!");

        return new TribeIdentifier(styleIdentifier, themeTribeId);
    }
}
