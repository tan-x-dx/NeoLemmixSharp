using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class LevelObjectiveDataComponentReader : ILevelDataReader
{
    private readonly Dictionary<ushort, string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelObjectivesDataSectionIdentifier;

    public LevelObjectiveDataComponentReader(Dictionary<ushort, string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}