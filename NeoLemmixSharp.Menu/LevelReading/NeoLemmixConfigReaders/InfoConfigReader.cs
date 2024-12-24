using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Menu.LevelPack;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class InfoConfigReader : NeoLemmixDataReader
{
    private readonly List<string> _messages = [];

    private string _title = string.Empty;
    private string _author = string.Empty;
    private string _version = string.Empty;
    private int _scrollerReadState = 0;

    public InfoConfigReader() : base(string.Empty)
    {
        RegisterTokenAction("TITLE", SetTitle);
        RegisterTokenAction("AUTHOR", SetAuthor);
        RegisterTokenAction("VERSION", SetVersion);
        RegisterTokenAction("PANEL", DoNothing);
        RegisterTokenAction("$SCROLLER", EnterScrollerSection);
        RegisterTokenAction("LINE", ReadMessage);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override bool BeginReading(ReadOnlySpan<char> line) => true;

    private void DoNothing(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetTitle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var levelTitle = line.TrimAfterIndex(secondTokenIndex).ToString();
        _title = levelTitle;
    }

    private void SetAuthor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var levelAuthor = line.TrimAfterIndex(secondTokenIndex).ToString();
        _author = levelAuthor;
    }

    private void SetVersion(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var levelVersion = line.TrimAfterIndex(secondTokenIndex).ToString();
        _version = levelVersion;
    }

    private void EnterScrollerSection(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _scrollerReadState = 1;
    }

    private void ReadMessage(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var message = line.TrimAfterIndex(secondTokenIndex).ToString();
        _messages.Add(message);
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_scrollerReadState == 1)
        {
            _scrollerReadState = 2;
        }
        else
        {
            throw new InvalidOperationException("Invalid scroller state!");
        }
    }

    public PackInfoData GetPackInfoData()
    {
        var result = new PackInfoData
        {
            Title = _title,
            Author = _author,
            Version = _version,

            Messages = _messages,
        };

        return result;
    }
}
