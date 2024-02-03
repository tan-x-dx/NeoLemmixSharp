using NeoLemmixSharp.Engine.Level.Skills;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public static class ReadingHelpers
{
    private const int MaxStackallocSize = 64;

    /// <summary>
    /// Returns the first two tokens from the initial span, where a token is defined as being a contiguous section of non-whitespace characters.
    /// If no such tokens exists, empty spans are returned.
    /// </summary>
    /// <param name="span">The source span</param>
    /// <param name="firstToken">The first span of non-whitespace characters</param>
    /// <param name="secondToken">The second span of non-whitespace characters</param>
    /// <param name="secondTokenIndex">The index of the second span. If no second span exists, -1 is returned</param>
    public static void GetTokenPair(
        ReadOnlySpan<char> span,
        out ReadOnlySpan<char> firstToken,
        out ReadOnlySpan<char> secondToken,
        out int secondTokenIndex)
    {
        firstToken = ReadOnlySpan<char>.Empty;
        secondTokenIndex = -1;
        secondToken = ReadOnlySpan<char>.Empty;

        if (span.Length == 0)
            return;

        // Safeguard against potential stack overflow.
        // Will almost certainly be a small buffer
        // allocated on the stack, but still...
        var bufferSize = span.Length + 1;
        Span<int> whitespaceIndexSpan = bufferSize > MaxStackallocSize
            ? new int[bufferSize]
            : stackalloc int[bufferSize];
        whitespaceIndexSpan[0] = -1;

        var i = 0;
        var j = 0;
        while (i < span.Length)
        {
            var c = span[i];
            if (char.IsWhiteSpace(c))
            {
                whitespaceIndexSpan[j++] = i;
            }

            i++;
        }

        whitespaceIndexSpan[j] = span.Length;

        i = 0;
        j++;

        var doFirstToken = true;
        var x0 = -1;
        var x1 = whitespaceIndexSpan[0];

        while (i++ < j)
        {
            var start = 1 + x0;
            var length = x1 - start;
            if (length > 0)
            {
                if (doFirstToken)
                {
                    firstToken = span.Slice(start, length);
                    doFirstToken = false;
                }
                else
                {
                    secondToken = span.Slice(start, length);
                    secondTokenIndex = start;
                    return;
                }
            }

            x0 = x1;
            x1 = whitespaceIndexSpan[i];
        }
    }

    /// <summary>
    /// Returns a sub-span starting at the given index, and trimming all leading and trailing whitespace.
    /// </summary>
    /// <param name="span">The original span</param>
    /// <param name="startIndex">The first index the result can start from</param>
    /// <returns>A ReadOnlySpan of trimmed-whitespace</returns>
    public static ReadOnlySpan<char> TrimAfterIndex(ReadOnlySpan<char> span, int startIndex)
    {
        if (startIndex < 0 || startIndex >= span.Length)
            return ReadOnlySpan<char>.Empty;

        var endIndex = span.Length - 1;
        while (endIndex > startIndex)
        {
            var c = span[endIndex];
            if (!char.IsWhiteSpace(c))
                break;

            endIndex--;
        }

        if (startIndex == endIndex)
            return ReadOnlySpan<char>.Empty;

        while (startIndex < endIndex)
        {
            var c = span[startIndex];
            if (!char.IsWhiteSpace(c))
                break;

            startIndex++;
        }

        return span[startIndex..(1 + endIndex)];
    }

    /// <summary>
    /// Parses a span into an unsigned integral type. The input must be hexadecimal format, and the leading characters may be hex signifiers.
    /// Valid leading characters may be "0x...", "0X...", "x...", "X..."
    /// </summary>
    /// <typeparam name="TNumber">The integral type to be parsed</typeparam>
    /// <param name="token">A sequence of characters representing a hexadecimal number</param>
    /// <returns>An unsigned integral type</returns>
    public static TNumber ParseHex<TNumber>(ReadOnlySpan<char> token)
        where TNumber : struct, IUnsignedNumber<TNumber>
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

    /// <summary>
    /// Helper method to prevent unnecessary allocations of empty strings
    /// </summary>
    /// <param name="span">The input span to be converted</param>
    /// <returns>A string representation of the input</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetString(this ReadOnlySpan<char> span)
    {
        return span.IsEmpty
            ? string.Empty
            : span.ToString();
    }

    public static bool GetSkillByName(
        ReadOnlySpan<char> token,
        IEqualityComparer<char> charEqualityComparer,
        out LemmingSkill lemmingSkill)
    {
        foreach (var item in LemmingSkill.AllItems)
        {
            var skillName = item.LemmingSkillName.AsSpan();
            if (skillName.SequenceEqual(token, charEqualityComparer))
            {
                lemmingSkill = item;
                return true;
            }
        }

        lemmingSkill = null!;
        return false;
    }

    /// <summary>
    /// Returns a (possibly null) reference to a <typeparamref name="TValue" />. A key is constructed based on the <paramref name="currentStyle" /> and <paramref name="piece" /> parameters,
    /// and either a new entry is created, or the existing entry is returned. NOTE: the returned reference may be null, and is expected to be initialised by the caller!
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="currentStyle">The style type</param>
    /// <param name="piece">The piece type</param>
    /// <param name="dictionary">The dictionary to look in</param>
    /// <param name="exists">When this method returns, contains <see langword="true" /> if the constructed key already existed in the dictionary, and <see langword="false" /> if a new entry was added.</param>
    /// <returns>A reference to a <typeparamref name="TValue" /> in the specified dictionary.</returns>
    public static ref TValue? GetArchetypeDataRef<TValue>(
        ReadOnlySpan<char> currentStyle,
        ReadOnlySpan<char> piece,
        Dictionary<string, TValue> dictionary,
        out bool exists)
        where TValue : class
    {
        var currentStyleLength = currentStyle.Length;

        // Safeguard against potential stack overflow.
        // Will almost certainly be a small buffer
        // allocated on the stack, but still...
        var bufferSize = currentStyleLength + piece.Length + 1;
        Span<char> archetypeDataKeySpan = bufferSize > MaxStackallocSize
            ? new char[bufferSize]
            : stackalloc char[bufferSize];

        currentStyle.CopyTo(archetypeDataKeySpan);
        piece.CopyTo(archetypeDataKeySpan[(currentStyleLength + 1)..]);
        archetypeDataKeySpan[currentStyleLength] = ':';

        var archetypeDataKey = archetypeDataKeySpan.ToString();

        return ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, archetypeDataKey, out exists);
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
        throw new InvalidDataException(
            $"Unknown token when parsing {identifierToken}: [{firstToken}] line: \"{line}\"");
    }
}