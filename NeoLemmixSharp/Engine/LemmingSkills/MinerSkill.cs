﻿using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class MinerSkill : LemmingSkill
{
    public static MinerSkill Instance { get; } = new();

    private MinerSkill()
    {
    }

    public override int LemmingSkillId => 13;
    public override string LemmingSkillName => "miner";
    public override bool IsPermanentSkill => false;

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
               && !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemming.LevelPosition,
                       lemming.FacingDirection.DeltaX))
                   .IsIndestructible(lemming.Orientation, lemming.FacingDirection, lemming.CurrentAction);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        MinerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}