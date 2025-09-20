using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Rendering.Text;

public static class TextRenderingHelpers
{
    private const int ZeroCharAsInt = '0';

    public static void WriteDigits(Span<char> span, int n, char blankCharValue = ' ')
    {
        var value = n;

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
    public static char DigitToChar(int digit) => (char)(digit | ZeroCharAsInt);

    public static int GetNumberStringLength(int n)
    {
        var negativeSign = 0;
        if (n < 0)
        {
            negativeSign = 1;
            n = -n;
        }

        var simpleLog10 = n switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            _ => 4 // We're not going to be dealing with numbers above a few thousand
        };

        return simpleLog10 + negativeSign;
    }
}
