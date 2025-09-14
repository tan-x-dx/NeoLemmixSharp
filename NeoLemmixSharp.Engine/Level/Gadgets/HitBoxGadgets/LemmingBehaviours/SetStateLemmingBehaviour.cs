using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetStateLemmingBehaviour : LemmingBehaviour
{
    private readonly ILemmingState _lemmingStateChanger;
    private readonly SetStateType _type;

    public SetStateLemmingBehaviour(
        ILemmingState lemmingStateChanger,
        SetStateType type)
        : base(LemmingBehaviourType.SetLemmingState)
    {
        _lemmingStateChanger = lemmingStateChanger;
        _type = type;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        var lemmingState = lemming.State;

        if (_type == SetStateType.Toggle)
        {
            _lemmingStateChanger.ToggleLemmingState(lemmingState);
            return;
        }

        _lemmingStateChanger.SetLemmingState(lemmingState, _type != SetStateType.Clear);
    }

    public enum SetStateType
    {
        Clear,
        Set,
        Toggle
    }

    private const int NumberOfEnumValues = 3;

    public static SetStateType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<SetStateType>(rawValue, NumberOfEnumValues);
}

public sealed class ZombieStateChanger : ILemmingState
{
    public static readonly ZombieStateChanger Instance = new();

    public StateType LemmingStateType => StateType.ZombieState;

    private ZombieStateChanger()
    {
    }

    public void SetLemmingState(LemmingState lemmingState, bool status) => lemmingState.IsZombie = status;
    public void ToggleLemmingState(LemmingState lemmingState) => lemmingState.IsZombie = !lemmingState.IsZombie;
    public bool IsApplied(LemmingState lemmingState) => lemmingState.IsZombie;
}

public sealed class NeutralStateChanger : ILemmingState
{
    public static readonly NeutralStateChanger Instance = new();

    public StateType LemmingStateType => StateType.NeutralState;

    private NeutralStateChanger()
    {
    }

    public void SetLemmingState(LemmingState lemmingState, bool status) => lemmingState.IsNeutral = status;
    public void ToggleLemmingState(LemmingState lemmingState) => lemmingState.IsNeutral = !lemmingState.IsNeutral;
    public bool IsApplied(LemmingState lemmingState) => lemmingState.IsNeutral;
}
