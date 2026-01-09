using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

            var requiredLength = GetNumberStringLengthBig((uint)n);

            var destSpan = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(destination), c), requiredLength);
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
            result += GetNumberStringLengthBig((uint)n);
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
        var length = GetNumberStringLengthSmall(valueToWrite);
        var digitsWritten = 0;

        do
        {
            (valueToWrite, uint rem) = Math.DivRem(valueToWrite, 10);

            pointer[length] = DigitToChar(rem);
            length--;
            digitsWritten++;
        }
        while (length >= 0);

        return digitsWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char DigitToChar(uint digit) => (char)(digit | ZeroCharAsUint);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNumberStringLengthSmall(uint n)
    {
        var result = 1;
        if (n >= 10) result++;
        if (n >= 100) result++;
        if (n >= 1000) result++;

        return result;
    }

    private static int GetNumberStringLengthBig(uint n)
    {
        if (n < 10) return 1;
        if (n < 100) return 2;
        if (n < 1000) return 3;
        if (n < 10000) return 4;
        if (n < 100000) return 5;
        if (n < 1000000) return 6;
        if (n < 10000000) return 7;
        if (n < 100000000) return 8;
        if (n < 1000000000) return 9;

        return 10;
    }
}
