using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DiggerSkill : LemmingSkill
{
    public static readonly DiggerSkill Instance = new();

    private DiggerSkill()
        : base(
            LemmingSkillType.DiggerSkill,
            LemmingSkillConstants.DiggerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        LevelScreen.GadgetManager.GetAllGadgetsNearPosition(lemming.AnchorPosition, out var gadgetsNearRegion);

        return SkillIsAssignableToCurrentAction(lemming) &&
               !PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, DiggerAction.DestructionMask, lemming.AnchorPosition);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        DiggerAction.TransitionLemmingToAction(lemming, false);
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
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
