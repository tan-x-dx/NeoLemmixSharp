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

    public static int QuickLog10(int n) => n switch
    {
        < 10 => 1,
        < 100 => 2,
        < 1000 => 3,
        _ => 4
    };
}