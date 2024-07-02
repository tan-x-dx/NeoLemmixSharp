using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class GadgetComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public GadgetComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }


    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x3D, 0x98];
        return sectionIdentifier;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}