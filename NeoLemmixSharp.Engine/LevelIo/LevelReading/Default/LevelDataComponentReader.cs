using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;

public abstract class LevelDataComponentReader
{
    private readonly LevelFileSectionIdentifier _sectionIdentifier;
    public bool AlreadyUsed { get; protected set; }

    protected LevelDataComponentReader(LevelFileSectionIdentifier sectionIdentifier)
    {
        _sectionIdentifier = sectionIdentifier;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        var index = (int)_sectionIdentifier;
        index <<= 1;

        return LevelReadWriteHelpers
        .LevelDataSectionIdentifierBytes
        .Slice(index, LevelReadWriteHelpers.NumberOfBytesForLevelSectionIdentifier);
    }

    public abstract void ReadSection(RawLevelFileData rawFileData, LevelData levelData);
}