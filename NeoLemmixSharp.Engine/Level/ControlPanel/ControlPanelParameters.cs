global using ControlPanelParameterSet = NeoLemmixSharp.Common.Util.Collections.SimpleSet<NeoLemmixSharp.Engine.Level.ControlPanel.ControlPanelParameters>;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public enum ControlPanelParameters
{
    ShowPauseButton,
    ShowNukeButton,
    ShowFastForwardsButton,
    ShowRestartButton,
    ShowFrameNudgeButtons,
    ShowDirectionSelectButtons,
    ShowClearPhysicsAndReplayButton,
    ShowReleaseRateButtonsIfPossible,
    EnableClassicModeSkillsIfPossible,
    RemoveSkillAssignPaddingButtons,
    ShowSpawnInterval
}