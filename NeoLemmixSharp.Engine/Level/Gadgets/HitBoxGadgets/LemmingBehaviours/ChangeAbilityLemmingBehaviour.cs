using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class ChangeAbilityLemmingBehaviour : LemmingBehaviour
{
    private readonly ILemmingAbilityChanger _lemmingAbilityChanger;
    private readonly ChangeAbilityType _type;

    public ChangeAbilityLemmingBehaviour(
        ILemmingAbilityChanger lemmingAbilityChanger,
        ChangeAbilityType type)
        : base(LemmingBehaviourType.ChangeLemmingAbility)
    {
        _lemmingAbilityChanger = lemmingAbilityChanger;
        _type = type;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        var lemmingState = lemming.State;

        if (_type == ChangeAbilityType.Toggle)
        {
            _lemmingAbilityChanger.ToggleLemmingAbility(lemmingState);
            return;
        }

        _lemmingAbilityChanger.SetLemmingAbility(lemmingState, _type != ChangeAbilityType.Clear);
    }

    public enum ChangeAbilityType
    {
        Clear,
        Set,
        Toggle,

        VALUE_MAX
    }

    private const int NumberOfEnumValues = (int)ChangeAbilityType.VALUE_MAX;

    public static ChangeAbilityType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<ChangeAbilityType>(rawValue, NumberOfEnumValues);
}

public sealed class ZombieStateChanger : ILemmingAbilityChanger
{
    public static readonly ZombieStateChanger Instance = new();

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.ZombieAbility;

    private ZombieStateChanger()
    {
    }

    public void SetLemmingAbility(LemmingState lemmingState, bool status) => lemmingState.IsZombie = status;
    public void ToggleLemmingAbility(LemmingState lemmingState) => lemmingState.IsZombie = !lemmingState.IsZombie;
    public bool LemmingHasAbility(LemmingState lemmingState) => lemmingState.IsZombie;
}

public sealed class NeutralStateChanger : ILemmingAbilityChanger
{
    public static readonly NeutralStateChanger Instance = new();

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.NeutralAbility;

    private NeutralStateChanger()
    {
    }

    public void SetLemmingAbility(LemmingState lemmingState, bool status) => lemmingState.IsNeutral = status;
    public void ToggleLemmingAbility(LemmingState lemmingState) => lemmingState.IsNeutral = !lemmingState.IsNeutral;
    public bool LemmingHasAbility(LemmingState lemmingState) => lemmingState.IsNeutral;
}
