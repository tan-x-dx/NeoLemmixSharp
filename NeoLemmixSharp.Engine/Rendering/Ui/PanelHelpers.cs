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
	public const int FrameNudgeButtonX = 4;
	public const int DirectionSelectButtonX = 5;
	public const int ClearPhysicsAndReplayButtonX = 6;
	public const int MinusButtonX = 7;
	public const int PlusButtonX = 8;

	public const int PanelBackgroundsY = 0;
	public const int ButtonIconsY = 1;

	public const int PaddingButtonX = 0;
	public const int PaddingButtonY = 2;

	public const int SkillIconMaskX = 1;
	public const int SkillIconMaskY = 2;

	public const int SkillAssignScrollLeftX = 2;
	public const int SkillAssignScrollRightX = 3;
	public const int SkillAssignScrollY = 2;

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