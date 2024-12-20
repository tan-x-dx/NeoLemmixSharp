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

    public override void BeginReading(ReadOnlySpan<char> line)
    {
        // Do nothing
    }

    private void ReadLevelFileName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelFileNames.Add(line.TrimAfterIndex(secondTokenIndex).ToString());
    }

    public List<string> GetLevelFileNames() => _levelFileNames;
}
