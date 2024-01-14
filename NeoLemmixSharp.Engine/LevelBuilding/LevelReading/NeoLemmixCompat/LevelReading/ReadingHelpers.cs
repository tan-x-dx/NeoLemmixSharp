using System.Globalization;
using System.Numerics;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public static class ReadingHelpers
{
    public static ReadOnlySpan<char> GetToken(ReadOnlySpan<char> span, int tokenIndex, out int indexOfFirstCharacter)
    {
        // Safeguard against potential stack overflow.
        // Will almost certainly be a small string
        // allocated on the stack, but still...
        Span<int> whitespaceIndexSpan = span.Length > 254
            ? new int[2 + span.Length]
            : stackalloc int[2 + span.Length];
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

    public static TNumber ParseUnsignedNumericalValue<TNumber>(ReadOnlySpan<char> token)
        where TNumber : struct, IUnsignedNumber<TNumber>, IParsable<TNumber>
    {
        // The standard parse methods can deal with hexadecimal, but
        // the initial "0x" part must be omitted. We deal with this here

        var startIndex = 0;
        if (token[0] == 'x')
        {
            startIndex = 1;
        }
        else if (token[1] == 'x')
        {
            startIndex = 2;
        }

        return TNumber.Parse(token[startIndex..], NumberStyles.AllowHexSpecifier, null);
    }

    public static bool TryGetWithSpan<TValue>(Dictionary<string, TValue> dictionary, ReadOnlySpan<char> testSpan, out TValue value)
    {
        // This is a really terrible way to use a dictionary,
        // but we only have a span to work with, so it's the
        // best we can do without allocating new strings
        foreach (var (key, result) in dictionary)
        {
            var keySpan = key.AsSpan();
            if (testSpan.SequenceEqual(keySpan))
            {
                value = result;
                return true;
            }
        }

        value = default!;
        return false;
    }
}