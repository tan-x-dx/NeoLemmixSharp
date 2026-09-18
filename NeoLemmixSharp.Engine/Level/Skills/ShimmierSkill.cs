using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class ShimmierSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        var lemmingManager = LevelScreen.LemmingManager;
        var currentActionType = lemming.CurrentActionType;

        var gadgetManager = LevelScreen.GadgetManager;
        if (currentActionType == LemmingActionType.ClimberAction)
        {
            var simulationLemming = lemmingManager.SimulateLemming(lemming, true);

            if (simulationLemming.CurrentActionType != LemmingActionType.SliderAction &&
                (simulationLemming.CurrentActionType != LemmingActionType.FallerAction ||
                 simulationLemming.FacingDirection == lemming.FacingDirection))
                return false;

            var simulationOrientation = simulationLemming.Orientation;
            var simulationPosition = simulationLemming.AnchorPosition;

            var gadgetTestRegion = new RectangularRegion(
                simulationPosition,
                simulationOrientation.MoveUp(simulationPosition, 9));
            gadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

            return PositionIsSolidToLemming(in gadgetsNearRegion, simulationLemming, simulationOrientation.MoveUp(simulationPosition, 9)) ||
                   PositionIsSolidToLemming(in gadgetsNearRegion, simulationLemming, simulationOrientation.MoveUp(simulationPosition, 8));
        }

        if (currentActionType == LemmingActionType.SliderAction ||
            currentActionType == LemmingActionType.DehoisterAction)
        {
            var oldActionType = lemming.CurrentActionType;

            var simulationLemming = lemmingManager.SimulateLemming(lemming, true);

            return simulationLemming.CurrentActionType != oldActionType &&
                   simulationLemming.FacingDirection == lemming.FacingDirection &&
                   (oldActionType != LemmingActionType.DehoisterAction || simulationLemming.CurrentActionType != LemmingActionType.SliderAction);
        }

        if (currentActionType != LemmingActionType.JumperAction)
            return LemmingSkillType.ShimmierSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.AnchorPosition;

        var gadgetTestRegion1 = new RectangularRegion(
            lemmingPosition,
            orientation.MoveUp(lemmingPosition, 12));
        gadgetManager.GetAllItemsNearRegion(gadgetTestRegion1, out var gadgetsNearRegion1);

        for (var i = 0; i < 5; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion1, lemming, orientation.MoveUp(lemmingPosition, 8 + i)) &&
                !PositionIsSolidToLemming(in gadgetsNearRegion1, lemming, orientation.MoveUp(lemmingPosition, 7 + i)))
                return true;
        }

        return LemmingSkillType.ShimmierSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        if (lemming.CurrentActionType is LemmingActionType.ClimberAction or
                                         LemmingActionType.SliderAction or
                                         LemmingActionType.JumperAction or
                                         LemmingActionType.DehoisterAction)
        {
            ShimmierAction.TransitionLemmingToAction(lemming, false);
        }
        else
        {
            ReacherAction.TransitionLemmingToAction(lemming, false);
        }
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
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.LasererAction;
    }
}
