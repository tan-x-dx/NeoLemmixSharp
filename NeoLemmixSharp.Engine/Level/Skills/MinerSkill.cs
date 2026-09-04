using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class MinerSkill : LemmingSkill
{
    public static readonly MinerSkill Instance = new();

    private MinerSkill()
        : base(
            LemmingSkillType.MinerSkill,
            LemmingSkillConstants.MinerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        LevelScreen.GadgetManager.GetAllGadgetsNearPosition(lemming.AnchorPosition, out var gadgetsNearRegion);

        return SkillIsAssignableToCurrentAction(lemming) &&
               !PositionIsIndestructibleToLemming(
                   in gadgetsNearRegion,
                   lemming,
                   MinerAction.DestructionMask,
                   lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX));
    }

    public override void AssignToLemming(Lemming lemming)
    {
        MinerAction.TransitionLemmingToAction(lemming, false);
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
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
