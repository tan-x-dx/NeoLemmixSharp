using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util;

public static class NumberFormattingHelpers
{
    private const uint ZeroCharAsUint = '0';

    public static void WriteDigits(Span<char> span, uint valueToWrite, char blankCharValue = ' ')
    {
        var length = span.Length;
        length--;

        if (length < 0)
            return;

        WriteDigit:
        (valueToWrite, uint rem) = Math.DivRem(valueToWrite, 10);

        Unsafe.Add(ref MemoryMarshal.GetReference(span), length) = DigitToChar(rem);
        length--;

        if (length < 0)
            return;

        if (valueToWrite == 0)
            goto WriteBlank;

        goto WriteDigit;

    WriteBlank:
        Unsafe.Add(ref MemoryMarshal.GetReference(span), length) = blankCharValue;
        length--;

        if (length >= 0)
            goto WriteBlank;
    }

    public static unsafe void WriteDigits(char* pointer, uint valueToWrite)
    {
        var length = GetNumberStringLength(valueToWrite);
        length--;

        do
        {
            (valueToWrite, uint rem) = Math.DivRem(valueToWrite, 10);

            pointer[length] = DigitToChar(rem);
            length--;
        }
        while (length >= 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char DigitToChar(uint digit) => (char)(digit | ZeroCharAsUint);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNumberStringLength(uint n)
    {
        var result = 1;
        if (n >= 10) result++;
        if (n >= 100) result++;
        if (n >= 1000) result++; // We're only going to be dealing with numbers less than ten thousand

        return result;
    }
}
