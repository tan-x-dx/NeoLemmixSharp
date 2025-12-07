using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class NumberFormattingHelpers
{
    private const uint ZeroCharAsUint = '0';

    public static void WriteDigits(Span<char> span, uint valueToWrite, char blankCharValue = ' ')
    {
        for (var i = span.Length - 1; i >= 0; i--)
        {
            if (valueToWrite > 0)
            {
                (uint div, uint rem) = Math.DivRem(valueToWrite, 10);

                span[i] = DigitToChar(rem);
                valueToWrite = div;
            }
            else
            {
                span[i] = blankCharValue;
            }
        }
    }

    public static unsafe void WriteDigits(char* pointer, uint valueToWrite)
    {
        var length = GetNumberStringLength(valueToWrite);
        length--;

        while (length >= 0)
        {
            (uint div, uint rem) = Math.DivRem(valueToWrite, 10);

            pointer[length] = DigitToChar(rem);
            valueToWrite = div;
            length--;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char DigitToChar(uint digit) => (char)(digit | ZeroCharAsUint);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNumberStringLength(uint n)
    {
        var result = 1;
        if (n >= 10) result++;
        if (n >= 100) result++;
        if (n >= 1000) result++; // We're going to be dealing with numbers less than ten thousand

        return result;
    }
}
