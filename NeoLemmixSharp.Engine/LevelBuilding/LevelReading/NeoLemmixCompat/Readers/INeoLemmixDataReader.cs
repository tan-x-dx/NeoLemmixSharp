﻿namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public interface INeoLemmixDataReader
{
    bool FinishedReading { get; }
    string IdentifierToken { get; }

    bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        var itemSpan = IdentifierToken.AsSpan();

        return itemSpan.SequenceEqual(token);
    }

    void BeginReading(ReadOnlySpan<char> line);
    bool ReadNextLine(ReadOnlySpan<char> line);
}

public abstract class NeoLemmixDataReader : INeoLemmixDataReader, IEqualityComparer<char>
{
    protected static readonly StringComparer OrdinalIgnoreCaseComparer = StringComparer.OrdinalIgnoreCase;

    protected readonly Dictionary<string, NxlvReadingHelpers.TokenAction> _tokenActions = new(OrdinalIgnoreCaseComparer);

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
        return firstToken.SequenceEqual(secondToken, this);
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

    bool IEqualityComparer<char>.Equals(char x, char y)
    {
        return char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
    }

    int IEqualityComparer<char>.GetHashCode(char obj)
    {
        return char.ToUpperInvariant(obj).GetHashCode();
    }
}