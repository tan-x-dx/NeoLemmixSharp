using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

public abstract class NeoLemmixDataReader
{
    protected delegate void TokenAction(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex);

    protected static readonly TokenAction DoNothing = (_, _, _) => { };

    public string IdentifierToken { get; }
    private readonly Dictionary<string, TokenAction> _tokenActions = new(StringComparer.OrdinalIgnoreCase);
    private readonly UnknownTokenBehaviour _unknownTokenBehaviour;

    public bool FinishedReading { get; protected set; }

    protected NeoLemmixDataReader(string identifierToken, UnknownTokenBehaviour unknownTokenBehaviour = UnknownTokenBehaviour.ThrowException)
    {
        IdentifierToken = identifierToken;
        _unknownTokenBehaviour = unknownTokenBehaviour;
    }

    protected void SetNumberOfTokens(int numberOfTokens) => _tokenActions.EnsureCapacity(numberOfTokens);

    protected void RegisterTokenAction(string token, TokenAction action) => _tokenActions.Add(token, action);

    public virtual bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        return Helpers.StringSpansMatch(token, IdentifierToken);
    }

    /// <summary>
    /// Initial processing. Return true if the first line should be reprocessed.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public abstract bool BeginReading(ReadOnlySpan<char> line);

    public virtual bool ReadNextLine(ReadOnlySpan<char> line)
    {
        return ProcessLineTokens(line);
    }

    protected bool ProcessLineTokens(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        return ProcessTokenPair(line, firstToken, secondToken, secondTokenIndex);
    }

    protected bool ProcessTokenPair(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> firstToken,
        ReadOnlySpan<char> secondToken,
        int secondTokenIndex)
    {
        var alternateLookup = _tokenActions.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(firstToken, out var tokenAction))
        {
            tokenAction(line, secondToken, secondTokenIndex);
        }
        else if (_unknownTokenBehaviour == UnknownTokenBehaviour.ThrowException)
        {
            NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
        }

        return false;
    }

    protected enum UnknownTokenBehaviour
    {
        ThrowException,
        IgnoreLine
    }
}
