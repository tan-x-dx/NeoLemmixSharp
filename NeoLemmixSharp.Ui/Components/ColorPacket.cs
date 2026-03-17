using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Ui.Components;

public struct ColorPacket
{
    public Color NormalColor;
    public Color MouseOverColor;
    public Color MousePressColor;
    public Color ActiveColor;

    public ColorPacket(Color color)
    {
        NormalColor = color;
        MouseOverColor = color;
        MousePressColor = color;
        ActiveColor = color;
    }

    public ColorPacket(Color normalColor, Color mouseOverColor, Color mousePressColor, Color activeColor)
    {
        NormalColor = normalColor;
        MouseOverColor = mouseOverColor;
        MousePressColor = mousePressColor;
        ActiveColor = activeColor;
    }
}

public static class ColorPacketHelpers
{
    extension(ColorPacket colorPacket)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Color GetColorFromState(ComponentState componentState)
        {
            var index = (int)componentState;
            index &= 3;

            Color* p = (Color*)&colorPacket;
            p += index;
            return *p;
        }
    }
}
