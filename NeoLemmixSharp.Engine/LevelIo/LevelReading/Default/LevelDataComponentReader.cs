using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;

public abstract class LevelDataComponentReader
{
    private readonly int _sectionIdentifierIndex;
    public bool AlreadyUsed { get; protected set; }

    protected LevelDataComponentReader(int sectionIdentifierIndex)
    {
        _sectionIdentifierIndex = sectionIdentifierIndex;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers
        .LevelDataSectionIdentifierBytes
        .Slice(_sectionIdentifierIndex, LevelReadWriteHelpers.NumberOfBytesForSectionIdentifier);
    public abstract void ReadSection(RawFileData rawFileData, LevelData levelData);
}