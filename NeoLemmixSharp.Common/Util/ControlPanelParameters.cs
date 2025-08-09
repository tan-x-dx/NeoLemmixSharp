using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

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

public readonly struct ControlPanelParameterHasher : IEnumIdentifierHelper<ControlPanelParameters, BitBuffer32>
{
    private const int NumberOfEnumValues = 12;

    public int NumberOfItems => NumberOfEnumValues;

    public static ControlPanelParameters GetEnumValue(uint rawValue) => Helpers.GetEnumValue<ControlPanelParameters>(rawValue, NumberOfEnumValues);

    [Pure]
    public int Hash(ControlPanelParameters item) => (int)item;
    [Pure]
    public ControlPanelParameters UnHash(int index) => (ControlPanelParameters)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<ControlPanelParameterHasher, BitBuffer32, ControlPanelParameters> CreateBitArraySet() => new(new ControlPanelParameterHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<ControlPanelParameterHasher, BitBuffer32, ControlPanelParameters, TValue> CreateBitArrayDictionary<TValue>() => new(new ControlPanelParameterHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}