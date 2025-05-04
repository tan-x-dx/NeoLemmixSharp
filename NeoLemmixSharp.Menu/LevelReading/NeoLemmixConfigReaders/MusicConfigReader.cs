using NeoLemmixSharp.Engine.LevelIo.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class MusicConfigReader : NeoLemmixDataReader
{
    private readonly List<string> _tracks = [];

    public MusicConfigReader() : base(string.Empty)
    {
        RegisterTokenAction("TRACK", ReadTrack);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override bool BeginReading(ReadOnlySpan<char> line) => true;

    private void ReadTrack(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var trackName = line[secondTokenIndex..].Trim().ToString();
        _tracks.Add(trackName);
    }

    public List<string> GetMusicData() => _tracks;
}
