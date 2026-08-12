using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly SwimmerSkill Instance = new();

    private SwimmerSkill()
        : base(
            LemmingSkillType.SwimmerSkill,
            LemmingSkillConstants.SwimmerSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.SwimmerAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsSwimmer = true;
        if (lemming.CurrentActionType == LemmingActionType.DrownerAction)
        {
            SwimmerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(DrownerAction.Instance);
        result.UnionWith(ActionsThatCanBeAssignedPermanentSkill);

        return result;
    }

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsSwimmer = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsSwimmer = !lemming.IsSwimmer;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsSwimmer;
    }
}
