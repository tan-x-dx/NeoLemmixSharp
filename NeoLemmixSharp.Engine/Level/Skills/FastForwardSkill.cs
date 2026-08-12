using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FastForwardSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly FastForwardSkill Instance = new();

    private FastForwardSkill()
        : base(
            LemmingSkillType.FastForwardSkill,
            LemmingSkillConstants.FastForwardSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.FastForwardAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsPermanentFastForwards && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsPermanentFastForwards = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsPermanentFastForwards = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsPermanentFastForwards = !lemming.IsPermanentFastForwards;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsPermanentFastForwards;
    }
}
