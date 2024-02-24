global using ControlPanelParameterSet = NeoLemmixSharp.Common.Util.Collections.SimpleSet<NeoLemmixSharp.Engine.Level.ControlPanel.ControlPanelParameters>;
using NeoLemmixSharp.Common.Util.Collections;

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

public static class ControlPanelParameterHelpers
{
    private const int NumberOfControlPanelParameters = 11;

    private sealed class ControlPanelParametersHasher : IPerfectHasher<ControlPanelParameters>
    {
        public int NumberOfItems => NumberOfControlPanelParameters;

        public int Hash(ControlPanelParameters item) => (int)item;

        public ControlPanelParameters UnHash(int index) => (ControlPanelParameters)index;
    }

    public static ControlPanelParameterSet CreateSimpleSet() => new(new ControlPanelParametersHasher());
}