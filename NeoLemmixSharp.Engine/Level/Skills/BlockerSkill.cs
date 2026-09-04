using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BlockerSkill : LemmingSkill
{
    public static readonly BlockerSkill Instance = new();

    private BlockerSkill()
        : base(
            LemmingSkillType.BlockerSkill,
            LemmingSkillConstants.BlockerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return SkillIsAssignableToCurrentAction(lemming) && LevelScreen.LemmingManager.CanAssignBlocker(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        BlockerAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
