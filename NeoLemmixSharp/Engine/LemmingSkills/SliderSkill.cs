namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class SliderSkill : LemmingSkill
{
    public SliderSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 16;
    public override string LemmingSkillName => "slider";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsSlider && lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsSlider = true;

        return true;
    }
}