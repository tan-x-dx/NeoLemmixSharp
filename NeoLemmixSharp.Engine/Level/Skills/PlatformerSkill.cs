using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class PlatformerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPostion = lemming.AnchorPosition;

        var levelRegion = new RectangularRegion(
            orientation.Move(lemmingPostion, new(-5, 2)),
            orientation.Move(lemmingPostion, new(5, -2)));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(levelRegion, out var gadgetsNearLemming);

        return LemmingSkillType.PlatformerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType) &&
               PlatformerAction.LemmingCanPlatform(lemming, in gadgetsNearLemming);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        PlatformerAction.TransitionLemmingToAction(lemming, false);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.LasererAction;
    }
}
