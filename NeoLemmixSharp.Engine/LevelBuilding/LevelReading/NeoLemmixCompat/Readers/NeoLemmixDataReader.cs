using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public abstract class NeoLemmixDataReader
{
    protected readonly Dictionary<string, NxlvReadingHelpers.TokenAction> _tokenActions = new(StringComparer.OrdinalIgnoreCase);

    public bool FinishedReading { get; protected set; }
    public string IdentifierToken { get; }

    protected NeoLemmixDataReader(string identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    protected void RegisterTokenAction(string token, NxlvReadingHelpers.TokenAction action)
    {
        _tokenActions.Add(token, action);
    }

    protected bool TokensMatch(
        ReadOnlySpan<char> firstToken,
        ReadOnlySpan<char> secondToken)
    {
        return firstToken.SequenceEqual(secondToken, Helpers.CaseInvariantCharEqualityComparer);
    }

    public virtual bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        return TokensMatch(token, IdentifierToken);
    }

    public abstract void BeginReading(ReadOnlySpan<char> line);

    public virtual bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        var alternateLookup = _tokenActions.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(firstToken, out var tokenAction))
        {
            tokenAction(line, secondToken, secondTokenIndex);
        }
        else
        {
            NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
        }

        return false;
    }
}