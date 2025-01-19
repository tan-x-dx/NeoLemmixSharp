using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Skills;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public static class NxlvReadingHelpers
{
    public delegate void TokenAction(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex);

    public const int MaxStackallocSize = 64;

    /// <summary>
    /// Returns the first two tokens from the initial span, where a token is defined as being a contiguous section of non-whitespace characters.
    /// If no such tokens exists, empty spans are returned.
    /// </summary>
    /// <param name="line">The source span</param>
    /// <param name="firstToken">The first span of non-whitespace characters</param>
    /// <param name="secondToken">The second span of non-whitespace characters</param>
    /// <param name="secondTokenIndex">The index of the second span. If no second span exists, -1 is returned</param>
    public static void GetTokenPair(
        ReadOnlySpan<char> line,
        out ReadOnlySpan<char> firstToken,
        out ReadOnlySpan<char> secondToken,
        out int secondTokenIndex)
    {
        var tokenIterator = new TokenEnumerator(line);

        if (!tokenIterator.MoveNext())
        {
            firstToken = ReadOnlySpan<char>.Empty;
            secondToken = ReadOnlySpan<char>.Empty;
            secondTokenIndex = -1;
            return;
        }

        firstToken = tokenIterator.Current;

        if (!tokenIterator.MoveNext())
        {
            secondToken = ReadOnlySpan<char>.Empty;
            secondTokenIndex = -1;
            return;
        }

        secondToken = tokenIterator.Current;
        secondTokenIndex = tokenIterator.CurrentSpanStart;
    }

    /// <summary>
    /// Parses a span into an unsigned integral type. The input must be hexadecimal format, and the leading characters may be hex signifiers.
    /// Valid leading characters may be "0x...", "0X...", "x...", "X..."
    /// </summary>
    /// <typeparam name="TNumber">The integral type to be parsed</typeparam>
    /// <param name="token">A sequence of characters representing a hexadecimal number</param>
    /// <returns>An unsigned integral type</returns>
    public static TNumber ParseHex<TNumber>(ReadOnlySpan<char> token)
        where TNumber : unmanaged, IUnsignedNumber<TNumber>
    {
        // The standard parse methods can deal with hexadecimal, but
        // the initial "0x" part must be omitted. We deal with this here

        var startIndex = 0;
        if (token[0] == 'x' || token[0] == 'X')
        {
            startIndex = 1;
        }
        else if (token[1] == 'x' || token[1] == 'X')
        {
            startIndex = 2;
        }

        return TNumber.Parse(token[startIndex..], NumberStyles.AllowHexSpecifier, null);
    }

    public static bool TryGetSkillByName(
        ReadOnlySpan<char> token,
        [MaybeNullWhen(false)] out LemmingSkill lemmingSkill)
    {
        foreach (var item in LemmingSkill.AllItems)
        {
            var skillName = item.LemmingSkillName.AsSpan();
            if (skillName.Equals(token, StringComparison.OrdinalIgnoreCase))
            {
                lemmingSkill = item;
                return true;
            }
        }

        lemmingSkill = null;
        return false;
    }

    /// <summary>
    /// NeoLemmix data strings may be prefixed with a '#', indicating a comment
    /// </summary>
    /// <param name="line">The line to test</param>
    /// <returns>Whether the input is blank or a comment</returns>
    public static bool LineIsBlankOrComment(ReadOnlySpan<char> line)
    {
        foreach (var c in line)
        {
            if (char.IsWhiteSpace(c))
                continue;
            return c == '#';
        }

        return true;
    }

    [DoesNotReturn]
    public static void ThrowUnknownTokenException(
        ReadOnlySpan<char> identifierToken,
        ReadOnlySpan<char> firstToken,
        ReadOnlySpan<char> line)
    {
        throw new InvalidOperationException(
            $"Unknown token when parsing {identifierToken}: [{firstToken}] line: \"{line}\"");
    }

    [DoesNotReturn]
    public static T ThrowUnknownTokenException<T>(
        ReadOnlySpan<char> identifierToken,
        ReadOnlySpan<char> firstToken,
        ReadOnlySpan<char> line)
    {
        throw new InvalidOperationException(
            $"Unknown token when parsing {identifierToken}: [{firstToken}] line: \"{line}\"");
    }
}