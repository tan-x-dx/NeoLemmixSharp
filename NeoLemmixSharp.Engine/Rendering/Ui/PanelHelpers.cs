using Microsoft.Xna.Framework;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public static class PanelHelpers
{
    public const int ControlPanelButtonPixelWidth = 16;
    public const int ControlPanelButtonPixelHeight = 23;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle GetRectangleForCoordinates(int x, int y)
    {
        return new Rectangle(
            x * ControlPanelButtonPixelWidth,
            y * ControlPanelButtonPixelHeight,
            ControlPanelButtonPixelWidth,
            ControlPanelButtonPixelHeight);
    }
}