using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DisarmerSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly DisarmerSkill Instance = new();

    private DisarmerSkill()
        : base(
            LemmingSkillType.DisarmerSkill,
            LemmingSkillConstants.DisarmerSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.DisarmerAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsDisarmer && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsDisarmer = true;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsDisarmer = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsDisarmer = !lemming.IsDisarmer;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsDisarmer;
    }
}
