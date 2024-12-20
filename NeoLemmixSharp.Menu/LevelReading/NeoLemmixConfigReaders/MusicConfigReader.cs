using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class MusicConfigReader : NeoLemmixDataReader
{
    private readonly List<string> _tracks = [];

    public MusicConfigReader() : base(string.Empty)
    {
        RegisterTokenAction("TRACK", ReadTrack);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override void BeginReading(ReadOnlySpan<char> line)
    {
        // Do nothing
    }

    private void ReadTrack(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var trackName = line.TrimAfterIndex(secondTokenIndex).ToString();
        _tracks.Add(trackName);
    }

    public List<string> GetMusicData() => _tracks;
}
