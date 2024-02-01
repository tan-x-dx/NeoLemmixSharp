using NeoLemmixSharp.Engine.Level.Skills;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public static class ReadingHelpers
{
    private const int MaxStackallocSize = 128;

    /// <summary>
    /// Returns the nth token from the initial span, where a token is defined as being a contiguous section of non-whitespace characters.
    /// If no such section exists for the given parameters, an empty span is returned, and the <paramref name="indexOfFirstCharacter" /> parameter is set to -1.
    /// </summary>
    /// <param name="span">The source span</param>
    /// <param name="tokenIndex">The 0-based index of the token to fetch</param>
    /// <param name="indexOfFirstCharacter">The index of the first character of the result token, inside the source span</param>
    /// <returns>A span of non-whitespace characters</returns>
    public static ReadOnlySpan<char> GetToken(ReadOnlySpan<char> span, int tokenIndex, out int indexOfFirstCharacter)
    {
        const int padding = 2;

        // Safeguard against potential stack overflow.
        // Will almost certainly be a small buffer
        // allocated on the stack, but still...
        var bufferSize = span.Length + padding;
        Span<int> whitespaceIndexSpan = bufferSize > MaxStackallocSize
            ? new int[bufferSize]
            : stackalloc int[bufferSize];
        whitespaceIndexSpan[0] = -1;

        var i = 0;
        var j = 1;
        while (i < span.Length)
        {
            var c = span[i];
            if (char.IsWhiteSpace(c))
            {
                whitespaceIndexSpan[j++] = i;
            }

            i++;
        }

        whitespaceIndexSpan = whitespaceIndexSpan[..(j + 1)];
        whitespaceIndexSpan[^1] = span.Length;

        i = 0;

        while (i < j)
        {
            var x0 = whitespaceIndexSpan[i];
            var x1 = whitespaceIndexSpan[i + 1];

            i++;

            if (x1 - x0 == 1)
                continue;

            if (tokenIndex-- == 0)
            {
                indexOfFirstCharacter = 1 + x0;
                return span.Slice(indexOfFirstCharacter, x1 - x0 - 1);
            }
        }

        indexOfFirstCharacter = -1;
        return ReadOnlySpan<char>.Empty;
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

    public static bool LineIsBlankOrComment(string line)
    {
        return string.IsNullOrWhiteSpace(line) || line[0] == '#';
    }
}