using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting;

public abstract class LevelDataComponentWriter
{
    private readonly int _sectionIdentifierIndex;

    protected LevelDataComponentWriter(int sectionIdentifierIndex)
    {
        _sectionIdentifierIndex = sectionIdentifierIndex;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers
        .LevelDataSectionIdentifierBytes
        .Slice(_sectionIdentifierIndex, LevelReadWriteHelpers.NumberOfBytesForSectionIdentifier);

    public abstract ushort CalculateNumberOfItemsInSection(LevelData levelData);

    public abstract void WriteSection(
        BinaryWriter writer,
        LevelData levelData);
}