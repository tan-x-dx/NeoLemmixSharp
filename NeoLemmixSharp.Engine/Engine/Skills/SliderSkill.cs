using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class SliderSkill : LemmingSkill
{
    public static SliderSkill Instance { get; } = new();

    private SliderSkill()
    {
    }

    public override int Id => GameConstants.SliderSkillId;
    public override string LemmingSkillName => "slider";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsSlider && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSlider = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}