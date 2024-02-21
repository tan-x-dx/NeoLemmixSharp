using Microsoft.Xna.Framework;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public static class PanelHelpers
{
    public const int ControlPanelButtonPixelWidth = 16;
    public const int ControlPanelButtonPixelHeight = 23;

    public const int PauseButtonX = 0;
    public const int NukeButtonX = 1;
    public const int FastForwardButtonX = 2;
    public const int RestartButtonX = 3;
    public const int FrameNudgeLeftButtonX = 4;
    public const int FrameNudgeRightButtonX = 5;
    public const int DirectionSelectLeftButtonX = 6;
    public const int DirectionSelectRightButtonX = 7;
    public const int ClearPhysicsButtonX = 8;
    public const int ReplayButtonX = 9;
    public const int MinusButtonX = 10;
    public const int PlusButtonX = 11;
    public const int SkillAssignScrollLeftX = 12;
    public const int SkillAssignScrollRightX = 13;

    public const int PaddingButtonX = 8;
    public const int PaddingButtonY = 0;

    public const int SkillIconTripleMaskX = 0;
    public const int SkillIconDoubleMaskX = 1;

    public const int PanelBackgroundY = 0;
    public const int ButtonIconsY = 1;
    public const int SkillIconMaskY = 2;

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