using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class DiggerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        LevelScreen.GadgetManager.GetAllGadgetsNearPosition(lemming.AnchorPosition, out var gadgetsNearRegion);

        return LemmingSkillType.DiggerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType) &&
               !PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, DiggerAction.DestructionMask, lemming.AnchorPosition);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        DiggerAction.TransitionLemmingToAction(lemming, false);
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
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.LasererAction;
    }
}
