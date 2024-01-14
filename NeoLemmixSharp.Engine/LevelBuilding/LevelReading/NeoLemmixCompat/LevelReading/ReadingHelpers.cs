using System.Globalization;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public static class ReadingHelpers
{
    public static ReadOnlySpan<char> GetToken(ReadOnlySpan<char> span, int tokenIndex, out int indexOfFirstCharacter)
    {
        var numberOfWhitespaces = 0;
        foreach (var c in span)
        {
            if (char.IsWhiteSpace(c))
            {
                numberOfWhitespaces++;
            }
        }

        Span<int> whitespaceIndexSpan = stackalloc int[2 + numberOfWhitespaces];
        whitespaceIndexSpan[0] = -1;
        whitespaceIndexSpan[^1] = span.Length;

        var i = 0;
        var j = 0;
        while (i < span.Length && j < numberOfWhitespaces)
        {
            var c = span[i];
            if (char.IsWhiteSpace(c))
            {
                whitespaceIndexSpan[++j] = i;
            }

            i++;
        }

        i = 0;

        while (i < whitespaceIndexSpan.Length - 1)
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

    public static ulong ReadUlong(ReadOnlySpan<char> token)
    {
        var leadingHexSpecifier = -1;
        if (token[0] == 'x')
        {
            leadingHexSpecifier = 0;
        }

        if (token[1] == 'x')
        {
            leadingHexSpecifier = 1;
        }

        return leadingHexSpecifier >= 0
            ? ulong.Parse(token[(1 + leadingHexSpecifier)..], NumberStyles.AllowHexSpecifier)
            : ulong.Parse(token);
    }

    public static int ReadInt(ReadOnlySpan<char> token)
    {
        return int.Parse(token);
    }

    public static uint ReadUint(ReadOnlySpan<char> token, bool parseAlpha)
    {
        var leadingHexSpecifier = -1;
        if (token[0] == 'x')
        {
            leadingHexSpecifier = 0;
        }

        if (token[1] == 'x')
        {
            leadingHexSpecifier = 1;
        }

        var result = leadingHexSpecifier >= 0
            ? uint.Parse(token[(1 + leadingHexSpecifier)..], NumberStyles.AllowHexSpecifier)
            : uint.Parse(token);

        if (parseAlpha)
            return result;

        return result | 0xff000000U;
    }

    public static bool TryGetWithSpan<TValue>(Dictionary<string, TValue> dictionary, ReadOnlySpan<char> testSpan, out TValue value)
    {
        foreach (var (key, result) in dictionary)
        {
            var keySpan = key.AsSpan();
            if (testSpan.SequenceEqual(keySpan))
            {
                value = result;
                return true;
            }
        }

        value = default;
        return false;
    }
}