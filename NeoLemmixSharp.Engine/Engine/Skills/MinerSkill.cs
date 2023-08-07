using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class MinerSkill : LemmingSkill
{
    public static MinerSkill Instance { get; } = new();

    private MinerSkill()
    {
    }

    public override int Id => GameConstants.MinerSkillId;
    public override string LemmingSkillName => "miner";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) &&
               !Terrain.PixelIsIndestructibleToLemming(
                   lemming,
                   MinerAction.Instance,
                   lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX));
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        MinerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}