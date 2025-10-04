using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Rendering.Text;

public static class TextRenderingHelpers
{
    private const uint ZeroCharAsUint = '0';

    public static void WriteDigits(Span<char> span, uint value, char blankCharValue = ' ')
    {
        for (var i = span.Length - 1; i >= 0; i--)
        {
            if (value > 0)
            {
                uint digit = value % 10;

                span[i] = DigitToChar(digit);
                value /= 10;
            }
            else
            {
                span[i] = blankCharValue;
            }
        }
    }

    public static unsafe void WriteDigits(char* pointer, int length, uint valueToWrite, char blankCharValue = ' ')
    {
        length--;
        var endPointer = pointer + length;

        while (pointer <= endPointer)
        {
            if (valueToWrite > 0)
            {
                uint digit = valueToWrite % 10;

                *endPointer = DigitToChar(digit);
                valueToWrite /= 10;
            }
            else
            {
                *endPointer = blankCharValue;
            }
            endPointer--;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char DigitToChar(uint digit) => (char)(digit | ZeroCharAsUint);

    public static int GetNumberStringLength(uint n)
    {
        var result = 1;
        if (n >= 10) result++;
        if (n >= 100) result++;
        if (n >= 1000) result++; // We're not going to be dealing with numbers above a few thousand

        return result;
    }
}
