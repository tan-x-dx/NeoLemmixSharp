﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class MinerSkill : LemmingSkill
{
    public static readonly MinerSkill Instance = new();

    private MinerSkill()
    {
    }

    public override int Id => LevelConstants.MinerSkillId;
    public override string LemmingSkillName => "miner";
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) &&
               !LevelScreen.TerrainManager.PixelIsIndestructibleToLemming(
                   lemming,
                   MinerAction.Instance,
                   lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX));
    }

    public override void AssignToLemming(Lemming lemming)
    {
        MinerAction.Instance.TransitionLemmingToAction(lemming, false);
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