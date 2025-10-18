using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

public abstract class NeoLemmixDataReader
{
    public string IdentifierToken { get; }
    private readonly Dictionary<string, NxlvReadingHelpers.TokenAction> _tokenActions = new(StringComparer.OrdinalIgnoreCase);
    private readonly UnknownTokenBehaviour _unknownTokenBehaviour;

    public bool FinishedReading { get; protected set; }

    protected NeoLemmixDataReader(string identifierToken, UnknownTokenBehaviour unknownTokenBehaviour = UnknownTokenBehaviour.ThrowException)
    {
        IdentifierToken = identifierToken;
        _unknownTokenBehaviour = unknownTokenBehaviour;
    }

    protected void SetNumberOfTokens(int numberOfTokens) => _tokenActions.EnsureCapacity(numberOfTokens);

    protected void RegisterTokenAction(string token, NxlvReadingHelpers.TokenAction action) => _tokenActions.Add(token, action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool TokensMatch(
        ReadOnlySpan<char> firstToken,
        ReadOnlySpan<char> secondToken)
    {
        return firstToken.Equals(secondToken, StringComparison.OrdinalIgnoreCase);
    }

    public virtual bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        return TokensMatch(token, IdentifierToken);
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
