using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Ui.Components;

public struct ColorPacket
{
    public Color NormalColor;
    public Color MouseOverColor;
    public Color MousePressColor;
    public Color ActiveColor;

    public ColorPacket(Color normalColor, Color mouseOverColor, Color mousePressColor, Color activeColor)
    {
        NormalColor = normalColor;
        MouseOverColor = mouseOverColor;
        MousePressColor = mousePressColor;
        ActiveColor = activeColor;
    }

    public Span<Color> AsSpan() => MemoryMarshal.CreateSpan(
        ref Unsafe.As<ColorPacket, Color>(ref this), 4);
}
