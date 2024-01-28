using NeoLemmixSharp.Engine.Level.Skills;
using System.Globalization;
using System.Numerics;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public static class ReadingHelpers
{
    public const int MaxStackallocSize = 256;

    public static ReadOnlySpan<char> GetToken(ReadOnlySpan<char> span, int tokenIndex, out int indexOfFirstCharacter)
    {
        const int padding = 2;

        // Safeguard against potential stack overflow.
        // Will almost certainly be a small buffer
        // allocated on the stack, but still...
        Span<int> whitespaceIndexSpan = span.Length > MaxStackallocSize - padding
            ? new int[padding + span.Length]
            : stackalloc int[padding + span.Length];
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
        where TNumber : struct, IUnsignedNumber<TNumber>, IParsable<TNumber>
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
}