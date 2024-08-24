using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class TextRenderingHelpers
{
    public static void WriteDigits(Span<int> span, int value, int blankCharValue = ' ')
    {
        for (var i = span.Length - 1; i >= 0; i--)
        {
            if (value > 0)
            {
                var digit = value % 10;

                span[i] = DigitToChar(digit);
                value /= 10;
            }
            else
            {
                span[i] = blankCharValue;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DigitToChar(int digit) => digit | '0';

    public static int GetNumberStringLength(int n)
    {
        int sign;

        if (n < 0)
        {
            sign = 1;
            n = -n;
        }
        else
        {
            sign = 0;
        }

        var numberOfDigits = n switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            _ => 4 // We're not going to be dealing with numbers above a few thousand
        };

        return numberOfDigits + sign;
    }
}