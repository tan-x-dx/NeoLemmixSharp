using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class LevelConfigReader : NeoLemmixDataReader
{
    private readonly List<string> _levelFileNames = [];

    public LevelConfigReader() : base(string.Empty)
    {
        RegisterTokenAction("LEVEL", ReadLevelFileName);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override bool BeginReading(ReadOnlySpan<char> line) => true;

    private void ReadLevelFileName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelFileNames.Add(line[secondTokenIndex..].Trim().ToString());
    }

    public List<string> GetLevelFileNames() => _levelFileNames;
}
