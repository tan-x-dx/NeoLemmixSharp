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
    private static int DigitToChar(int digit) => digit + '0';
}