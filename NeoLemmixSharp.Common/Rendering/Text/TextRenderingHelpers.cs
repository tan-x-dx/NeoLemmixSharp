using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Rendering.Text;

public static class TextRenderingHelpers
{
    private const uint ZeroCharAsUint = '0';

    public static void WriteDigits(Span<char> span, uint n, char blankCharValue = ' ')
    {
        uint value = n;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char DigitToChar(uint digit) => (char)(digit | ZeroCharAsUint);

    public static int GetNumberStringLength(uint n)
    {
        var simpleLog10 = n switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            _ => 4 // We're not going to be dealing with numbers above a few thousand
        };

        return simpleLog10;
    }
}
