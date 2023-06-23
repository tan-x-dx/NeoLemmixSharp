using NeoLemmixSharp.Engine.Actions;

namespace NeoLemmixSharp.Engine.Skills;

public sealed class MinerSkill : LemmingSkill
{
    public MinerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 13;
    public override string LemmingSkillName => "miner";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return (lemming.CurrentAction == WalkerAction.Instance ||
                lemming.CurrentAction == ShruggerAction.Instance ||
                lemming.CurrentAction == PlatformerAction.Instance ||
                lemming.CurrentAction == BuilderAction.Instance ||
                lemming.CurrentAction == StackerAction.Instance ||
                lemming.CurrentAction == BasherAction.Instance ||
                lemming.CurrentAction == FencerAction.Instance ||
                lemming.CurrentAction == DiggerAction.Instance ||
                lemming.CurrentAction == LasererAction.Instance)
               && !Terrain.PixelIsIndestructibleToLemming(lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX), lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        MinerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}