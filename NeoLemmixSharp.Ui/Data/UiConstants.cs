using NeoLemmixSharp.Ui.Components;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Ui.Data;

public static class UiConstants
{
    public const int StandardButtonHeight = 32;

    public const int StandardInset = 8;
    public const int TwiceStandardInset = StandardInset * 2;

    public const int RaisedRectangleBorder = 2;

    private const float FontGlyphWidth = 26f;
    internal const float FontScaleFactor = 2.0f;
    internal const float FontGlyphWidthMultiplier = FontGlyphWidth * FontScaleFactor;

    private static ReadOnlySpan<uint> RawDefaultColors =>
    [
        0xff444444,
        0xff666666,
        0xff888888,
        0xff006600
    ];

    public static ColorPacket RectangularButtonDefaultColours => Unsafe.As<uint, ColorPacket>(ref MemoryMarshal.GetReference(RawDefaultColors));
}
