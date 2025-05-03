using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting;

public abstract class LevelDataComponentWriter
{
    private readonly LevelFileSectionIdentifier _sectionIdentifier;

    protected LevelDataComponentWriter(LevelFileSectionIdentifier sectionIdentifier)
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

    public abstract ushort CalculateNumberOfItemsInSection(LevelData levelData);

    public abstract void WriteSection(
        BinaryWriter writer,
        LevelData levelData);
}