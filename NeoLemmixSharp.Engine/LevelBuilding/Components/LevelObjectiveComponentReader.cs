using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class LevelObjectiveComponentReader : ILevelDataReader
{
    private const int NumberOfBytesForMainLevelObjectiveData = 7;
    private const int NumberOfBytesPerSkillSetDatum = 3;
    private const int NumberOfBytesPerRequirementsDatum = 4;

    private readonly List<string> _stringIdLookup;

    public LevelObjectiveComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0xBE, 0xF4];
        return sectionIdentifier;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}