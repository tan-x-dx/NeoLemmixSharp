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
        if (_type == ChangeAbilityType.Toggle)
        {
            _lemmingAbilityChanger.ToggleLemmingAbility(lemming);
            return;
        }

        _lemmingAbilityChanger.SetLemmingAbility(lemming, _type != ChangeAbilityType.Clear);
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

    public void SetLemmingAbility(Lemming lemming, bool status) => lemming.IsZombie = status;
    public void ToggleLemmingAbility(Lemming lemming) => lemming.IsZombie = !lemming.IsZombie;
    public bool LemmingHasAbility(Lemming lemming) => lemming.IsZombie;
}

public sealed class NeutralStateChanger : ILemmingAbilityChanger
{
    public static readonly NeutralStateChanger Instance = new();

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.NeutralAbility;

    private NeutralStateChanger()
    {
    }

    public void SetLemmingAbility(Lemming lemming, bool status) => lemming.IsNeutral = status;
    public void ToggleLemmingAbility(Lemming lemming) => lemming.IsNeutral = !lemming.IsNeutral;
    public bool LemmingHasAbility(Lemming lemming) => lemming.IsNeutral;
}
