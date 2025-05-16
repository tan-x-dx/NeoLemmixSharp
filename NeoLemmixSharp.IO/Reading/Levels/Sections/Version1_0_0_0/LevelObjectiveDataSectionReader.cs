using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class LevelObjectiveDataSectionReader : LevelDataSectionReader
{
    private readonly List<string> _stringIdLookup;

    public LevelObjectiveDataSectionReader(
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelObjectivesDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}