using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public static class ReadingHelpers
{
    private const int MaxStackallocSize = 128;

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

    public static ref T? GetArchetypeDataRef<T>(
        ReadOnlySpan<char> currentStyle,
        ReadOnlySpan<char> piece,
        Dictionary<string, T> dictionary,
        out bool exists)
        where T : class, IStyleArchetypeData
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
}