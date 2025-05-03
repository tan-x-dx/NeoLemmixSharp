using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public abstract class LevelDataSectionReader
{
    public abstract LevelFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes()
    {
        var index = (int)SectionIdentifier;
        index <<= 1;

        return LevelReadWriteHelpers
            .LevelDataSectionIdentifierBytes
            .Slice(index, LevelReadWriteHelpers.NumberOfBytesForLevelSectionIdentifier);
    }

    public abstract void ReadSection(RawLevelFileData rawFileData, LevelData levelData);
}