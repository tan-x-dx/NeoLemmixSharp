using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class TerrainComponentReader : ILevelDataReader
{
    private const int EraseBitShift = 0;
    private const int NoOverwriteBitShift = 1;
    private const int TintBitShift = 2;

    private const int NumberOfBytesForMainTerrainData = 9;

    private readonly List<string> _stringIdLookup;

    public TerrainComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x60, 0xBB];
        return sectionIdentifier;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}