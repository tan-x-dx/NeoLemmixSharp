using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ShimmierSkill : LemmingSkill
{
    public static readonly ShimmierSkill Instance = new();

    private ShimmierSkill()
        : base(
            LemmingSkillType.ShimmierSkill,
            LemmingSkillConstants.ShimmierSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
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
            return SkillIsAssignableToCurrentAction(lemming);

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

        return SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
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
