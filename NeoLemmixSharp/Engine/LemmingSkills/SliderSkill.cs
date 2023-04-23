namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class SliderSkill : LemmingSkill
{
    public static SliderSkill Instance { get; } = new();

    private SliderSkill()
    {
    }

    public override int LemmingSkillId => 16;
    public override string LemmingSkillName => "slider";
    public override bool IsPermanentSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsSlider && LemmingActionCanBeAssignedPermanentSkill(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsSlider = true;

        return true;
    }
}