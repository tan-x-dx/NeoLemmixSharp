using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitBuffers;
using System.Diagnostics.Contracts;

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
    ShowSpawnIntervalButtonsIfPossible,
    ShowSpawnIntervalInsteadOfReleaseRate,
    EnableClassicModeSkillsIfPossible,
    RemoveSkillAssignPaddingButtons,
    ShowExpandedAthleteInformation
}

public readonly struct ControlPanelParameterHasher : IPerfectHasher<ControlPanelParameters>, IBitBufferCreator<BitBuffer32>
{
    public int NumberOfItems => 12;

    [Pure]
    public int Hash(ControlPanelParameters item) => (int)item;
    [Pure]
    public ControlPanelParameters UnHash(int index) => (ControlPanelParameters)index;

    [Pure]
    public static ControlPanelParameterSet CreateBitArraySet(bool fullSet = false) => new(new ControlPanelParameterHasher(), fullSet);
    [Pure]
    public static BitArrayDictionary<ControlPanelParameterHasher, BitBuffer32, ControlPanelParameters, TValue> CreateBitArrayDictionary<TValue>() => new(new ControlPanelParameterHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}