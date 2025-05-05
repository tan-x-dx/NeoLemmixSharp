using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Menu.LevelPack;
using static NeoLemmixSharp.Menu.LevelPack.PostViewMessageData;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class PostViewConfigReader : NeoLemmixDataReader
{
    private readonly List<PostViewMessageData> _messageData = [];

    private ResultType? _absoluteOrPercentage;
    private ParityType? _aboveOrBelow;
    private int? _numericalValue;
    private List<string>? _lines;

    public PostViewConfigReader() : base(string.Empty)
    {
        SetNumberOfTokens(4);

        RegisterTokenAction("$RESULT", EnterResultGroup);
        RegisterTokenAction("CONDITION", ParseCondition);
        RegisterTokenAction("LINE", ReadLine);
        RegisterTokenAction("$END", ExitResultGroup);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override bool BeginReading(ReadOnlySpan<char> line) => true;

    private void EnterResultGroup(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _absoluteOrPercentage = null;
        _aboveOrBelow = null;
        _numericalValue = null;
        _lines = [];
    }

    private void ParseCondition(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_numericalValue.HasValue)
            throw new InvalidOperationException("Invalid state!");

        var indexOfFirstNumericalChar = 0;
        var indexOfLastNumericalChar = secondToken.Length - 1;

        var firstChar = secondToken[0];
        if (firstChar == '-')
        {
            _aboveOrBelow = ParityType.Below;
            indexOfFirstNumericalChar = 1;
        }
        else if (firstChar == '+')
        {
            _aboveOrBelow = ParityType.Above;
            indexOfFirstNumericalChar = 1;
        }
        else
        {
            _aboveOrBelow = ParityType.Equal;
        }

        var lastChar = secondToken[^1];
        if (lastChar == '%')
        {
            _absoluteOrPercentage = ResultType.Percentage;
            indexOfLastNumericalChar--;
        }
        else
        {
            _absoluteOrPercentage = ResultType.Absolute;
        }

        var numericalPartSpan = secondToken.Slice(indexOfFirstNumericalChar, 1 + indexOfLastNumericalChar - indexOfFirstNumericalChar);
        _numericalValue = int.Parse(numericalPartSpan);
    }

    private void ReadLine(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var messageLine = line[secondTokenIndex..].Trim().ToString();

        _lines!.Add(messageLine);
    }

    private void ExitResultGroup(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var newMessage = new PostViewMessageData
        {
            AbsoluteOrPercentage = _absoluteOrPercentage!.Value,
            AboveOrBelow = _aboveOrBelow!.Value,
            NumericalValue = _numericalValue!.Value,
            Lines = _lines!
        };

        _messageData.Add(newMessage);
    }

    public List<PostViewMessageData> GetPostViewData() => _messageData;
}
