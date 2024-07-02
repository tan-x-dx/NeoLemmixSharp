using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class PrePlacedLemmingComponentReader : ILevelDataReader
{
    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0xFE, 0x77];
        return sectionIdentifier;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}