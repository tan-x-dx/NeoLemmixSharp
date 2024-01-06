using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelParameters
{
	public bool TimedBombers { get; } = true;

	public ControlPanelParameters ControlPanelParameters { get; }

	public LevelParameters(LevelData levelData)
	{
		ControlPanelParameters = GetControlPanelButtonAvailability(levelData);
	}

	private static ControlPanelParameters GetControlPanelButtonAvailability(LevelData levelData)
	{
		return ControlPanelParameters.ShowPauseButton |
			   ControlPanelParameters.ShowNukeButton |
			   ControlPanelParameters.ShowFastForwardsButton |
			   ControlPanelParameters.ShowRestartButton |
			   ControlPanelParameters.ShowFrameNudgeButtons |
			   ControlPanelParameters.ShowDirectionSelectButtons |
			   ControlPanelParameters.ShowClearPhysicsAndReplayButton |
			   ControlPanelParameters.ShowReleaseRateButtonsIfPossible;
	}

	[Pure]
	public int GetLemmingCountDownTimer(Lemming lemming)
	{
		if (TimedBombers)
			return lemming.IsFastForward
				? LevelConstants.DefaultFastForwardLemmingCountDownActionTicks
				: LevelConstants.DefaultCountDownActionTicks;

		return 1; // I.e. the next frame
	}
}