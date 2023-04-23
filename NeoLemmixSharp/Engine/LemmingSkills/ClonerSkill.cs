using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class ClonerSkill : LemmingSkill
{
    public static ClonerSkill Instance { get; } = new();

    private ClonerSkill()
    {
    }

    public override int LemmingSkillId => 5;
    public override string LemmingSkillName => "cloner";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == JumperAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance ||
               lemming.CurrentAction == SwimmerAction.Instance ||
               lemming.CurrentAction == GliderAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == AscenderAction.Instance ||
               lemming.CurrentAction == FallerAction.Instance ||
               lemming.CurrentAction == FloaterAction.Instance ||
               lemming.CurrentAction == DisarmerAction.Instance ||
               lemming.CurrentAction == ShimmierAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == ReacherAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}