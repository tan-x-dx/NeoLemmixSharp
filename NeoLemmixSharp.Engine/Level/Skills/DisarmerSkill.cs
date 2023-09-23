using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DisarmerSkill : LemmingSkill, IPermanentSkill
{
    public static DisarmerSkill Instance { get; } = new();

    private DisarmerSkill()
    {
    }

    public override int Id => Global.DisarmerSkillId;
    public override string LemmingSkillName => "disarmer";
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

    public void SetPermanentSkill(Lemming lemming, bool status)
    {
        lemming.State.IsDisarmer = status;
    }

    public void TogglePermanentSkill(Lemming lemming)
    {
        var isDisarmer = lemming.State.IsDisarmer;
        lemming.State.IsDisarmer = !isDisarmer;
    }
}