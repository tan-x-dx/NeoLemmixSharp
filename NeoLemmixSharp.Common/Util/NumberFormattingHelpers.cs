using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class NumberFormattingHelpers
{
    private const uint ZeroCharAsUint = '0';

    public const int Uint16NumberBufferLength = 5;
    public const int Uint32NumberBufferLength = 10;
    public const int Int32NumberBufferLength = Uint32NumberBufferLength + 1;

    public readonly ref struct FormatParameters(char openBracket, char separator, char closeBracket)
    {
        public static FormatParameters Default => new('(', ',', ')');

        public readonly char OpenBracket = openBracket;
        public readonly char Separator = separator;
        public readonly char CloseBracket = closeBracket;
    }

    public static bool TryFormatIntegerSpan(
        ReadOnlySpan<int> source,
        Span<char> destination,
        FormatParameters formatParameters,
        out int charsWritten)
    {
        var requiredCharLength = CalculateRequiredCharLength(source);
        if (destination.Length < requiredCharLength)
        {
            charsWritten = 0;
            return false;
        }

        destination.At(0) = formatParameters.OpenBracket;
        var c = 1;

        for (var i = 0; i < source.Length; i++)
        {
            var n = source[i];

            if (n < 0)
            {
                destination.At(c++) = '-';
                n = -n;
            }

            var requiredLength = GetNumberStringLength((uint)n);

            var destSpan = Helpers.Slice(destination, c, requiredLength);
            c += WriteDigits(destSpan, (uint)n);
            destination.At(c++) = formatParameters.Separator;
        }

        if (source.Length > 0)
            c--;

        destination.At(c++) = formatParameters.CloseBracket;
        charsWritten = c;
        return true;
    }

    private static int CalculateRequiredCharLength(ReadOnlySpan<int> source)
    {
        if (source.Length == 0)
            return 2;

        var result = source.Length + 1;
        for (var i = 0; i < source.Length; i++)
        {
            var n = source[i];
            if (n < 0)
            {
                result++;
                n = -n;
            }
            result += GetNumberStringLength((uint)n);
        }

        return result;
    }

    public static int WriteDigits(Span<char> span, uint valueToWrite, char blankCharValue = ' ')
    {
        var digitsWritten = 0;
        var length = span.Length;
        length--;

        if (length < 0)
            return digitsWritten;

        WriteDigit:
        (valueToWrite, uint rem) = Math.DivRem(valueToWrite, 10);

        span.At(length) = DigitToChar(rem);
        length--;
        digitsWritten++;

        if (length < 0)
            return digitsWritten;

        if (valueToWrite == 0)
            goto WriteBlank;

        goto WriteDigit;

    WriteBlank:
        span.At(length) = blankCharValue;
        length--;

        if (length >= 0)
            goto WriteBlank;
        return digitsWritten;
    }

    public static unsafe int WriteDigits(char* pointer, uint valueToWrite)
    {
        var digitsWritten = GetNumberStringLength(valueToWrite);
        var length = digitsWritten;
        length--;

        do
        {
            (valueToWrite, uint rem) = Math.DivRem(valueToWrite, 10);

            pointer[length] = DigitToChar(rem);
            length--;
        }
        while (length >= 0);

        return digitsWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char DigitToChar(uint digit) => (char)(digit | ZeroCharAsUint);

    public static int GetNumberStringLength(uint n)
    {
        if (n < 100000)
        { // 1 to 5
            if (n < 100)
            { // 1 or 2
                return n < 10 ? 1 : 2;
            }
            else
            { // 3, 4 or 5
                if (n < 1000)
                    return 3;

                return n < 10000 ? 4 : 5;
            }
        }
        else
        { // 6 to 7
            if (n < 10000000)
            { // 6 or 7
                return n < 1000000 ? 6 : 7;
            }
            else
            { // 8, 9 or 10
                if (n < 100000000)
                    return 8;

                return n < 1000000000 ? 9 : 10;
            }
        }
    }
}
