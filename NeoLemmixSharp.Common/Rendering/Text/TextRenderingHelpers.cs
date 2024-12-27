using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Rendering.Text;

public static class TextRenderingHelpers
{
    public static void WriteDigits(Span<char> span, int value, char blankCharValue = ' ')
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
    public static char DigitToChar(int digit) => (char)(digit | '0');

    public static int GetNumberStringLength(int n) => n switch
    {
        < -9999 => 6,
        < -999 => 5,
        < -99 => 4,
        < -9 => 3,
        < 0 => 2,
        < 10 => 1,
        < 100 => 2,
        < 1000 => 3,
        < 10000 => 4,
        _ => 5 // We're not going to be dealing with numbers above a few thousand
    };
}