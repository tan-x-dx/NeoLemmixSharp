using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class MinerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        LevelScreen.GadgetManager.GetAllGadgetsNearPosition(lemming.AnchorPosition, out var gadgetsNearRegion);

        return LemmingSkillType.MinerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType) &&
               !PositionIsIndestructibleToLemming(
                   in gadgetsNearRegion,
                   lemming,
                   MinerAction.DestructionMask,
                   lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX));
    }

    public static void AssignToLemming(Lemming lemming)
    {
        MinerAction.TransitionLemmingToAction(lemming, false);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.LasererAction;
    }
}
