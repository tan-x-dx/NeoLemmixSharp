using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class DisarmerSkill : LemmingSkill
{
    public static DisarmerSkill Instance { get; } = new();

    private DisarmerSkill()
    {
    }

    public override int Id => GameConstants.DisarmerSkillId;
    public override string LemmingSkillName => "disarmer";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsDisarmer && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsDisarmer = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}