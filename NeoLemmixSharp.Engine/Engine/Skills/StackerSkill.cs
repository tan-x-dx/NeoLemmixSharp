﻿using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public StackerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 17;
    public override string LemmingSkillName => "stacker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        // Get starting position for stacker
        lemming.StackLow = !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX), lemming);

        StackerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }
}