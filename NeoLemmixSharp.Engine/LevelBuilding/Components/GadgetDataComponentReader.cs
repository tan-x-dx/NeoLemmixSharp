using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class GadgetDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public GadgetDataComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }


    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.GadgetDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}