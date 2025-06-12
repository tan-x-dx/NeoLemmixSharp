using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class StringDataSectionReader : LevelDataSectionReader
{
    private readonly FileStringReader<RawLevelFileDataReader> _stringReader;

    public StringDataSectionReader(
        MutableStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection, true)
    {
        _stringReader = new FileStringReader<RawLevelFileDataReader>(stringIdLookup);
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        _stringReader.ReadSection(rawFileData, numberOfItemsInSection);
    }
}
