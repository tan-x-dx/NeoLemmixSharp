﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BlockerSkill : LemmingSkill
{
    public static readonly BlockerSkill Instance = new();

    private BlockerSkill()
        : base(
            LevelConstants.BlockerSkillId,
            LevelConstants.BlockerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) && LevelScreen.LemmingManager.CanAssignBlocker(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        BlockerAction.Instance.TransitionLemmingToAction(lemming, false);
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
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}