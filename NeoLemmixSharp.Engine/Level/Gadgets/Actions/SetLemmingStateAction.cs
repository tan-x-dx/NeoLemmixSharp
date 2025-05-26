using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SetLemmingStateAction : GadgetAction
{
    private readonly ILemmingStateChanger _lemmingStateChanger;
    private readonly SetStateType _type;

    public SetLemmingStateAction(ILemmingStateChanger lemmingStateChanger, SetStateType type)
        : base(GadgetActionType.SetLemmingState)
    {
        _lemmingStateChanger = lemmingStateChanger;
        _type = type;
    }

    public override void PerformAction(Lemming lemming)
    {
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

public sealed class ZombieStateChanger : ILemmingStateChanger
{
    public static readonly ZombieStateChanger Instance = new();

    public StateChangerType LemmingStateChangerType => StateChangerType.ZombieStateChanger;

    private ZombieStateChanger()
    {
    }

    public void SetLemmingState(LemmingState lemmingState, bool status) => lemmingState.IsZombie = status;
    public void ToggleLemmingState(LemmingState lemmingState) => lemmingState.IsZombie = !lemmingState.IsZombie;
    public bool IsApplied(LemmingState lemmingState) => lemmingState.IsZombie;
}

public sealed class NeutralStateChanger : ILemmingStateChanger
{
    public static readonly NeutralStateChanger Instance = new();

    public StateChangerType LemmingStateChangerType => StateChangerType.NeutralStateChanger;

    private NeutralStateChanger()
    {
    }

    public void SetLemmingState(LemmingState lemmingState, bool status) => lemmingState.IsNeutral = status;
    public void ToggleLemmingState(LemmingState lemmingState) => lemmingState.IsNeutral = !lemmingState.IsNeutral;
    public bool IsApplied(LemmingState lemmingState) => lemmingState.IsNeutral;
}
