using NeoLemmixSharp.Engine.Actions;

namespace NeoLemmixSharp.Engine.Skills;

public sealed class ShimmierSkill : LemmingSkill
{
    public ShimmierSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 15;
    public override string LemmingSkillName => "shimmier";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance ||
            lemming.CurrentAction == JumperAction.Instance ||
            lemming.CurrentAction == DehoisterAction.Instance)
        {
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else
        {
            ReacherAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}