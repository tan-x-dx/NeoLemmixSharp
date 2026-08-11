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
        return !lemming.State.IsPermanentFastForwards && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsPermanentFastForwards = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(LemmingState lemmingState, bool status)
    {
        lemmingState.IsPermanentFastForwards = status;
    }

    public void ToggleLemmingAbility(LemmingState lemmingState)
    {
        lemmingState.IsPermanentFastForwards = !lemmingState.IsPermanentFastForwards;
    }

    public bool LemmingHasAbility(LemmingState lemmingState)
    {
        return lemmingState.IsPermanentFastForwards;
    }
}
