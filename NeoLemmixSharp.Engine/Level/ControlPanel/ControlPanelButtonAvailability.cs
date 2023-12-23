namespace NeoLemmixSharp.Engine.Level.ControlPanel;

[Flags]
public enum ControlPanelButtonAvailability
{
	ShowPauseButton = 1 << 0,
	ShowNukeButton = 1 << 1,
	ShowFastForwardsButton = 1 << 2,
	ShowRestartButton = 1 << 3,
	ShowFrameNudgeButtons = 1 << 4,
	ShowDirectionSelectButtons = 1 << 5,
	ShowClearPhysicsAndReplayButton = 1 << 6,
	ShowReleaseRateButtonsIfPossible = 1 << 7,
	EnableClassicModeSkillsIfPossible = 1 << 8,
	RemoveExcessSkillAssignButtons = 1 << 9,
}

public static class ControlPanelButtonAvailabilityHelpers
{
	public static bool HasFlag(this ControlPanelButtonAvailability item, ControlPanelButtonAvailability test)
	{
		return (item & test) != 0;
	}
}