using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FloaterSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly FloaterSkill Instance = new();

    private FloaterSkill()
        : base(
            LemmingSkillType.FloaterSkill,
            LemmingSkillConstants.FloaterSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.FloaterAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasSpecialFallingBehaviour && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsFloater = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsFloater = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsFloater = !lemming.IsFloater;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsFloater;
    }
}
