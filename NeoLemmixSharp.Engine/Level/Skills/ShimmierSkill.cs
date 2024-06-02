using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ShimmierSkill : LemmingSkill
{
    public static readonly ShimmierSkill Instance = new();

    private ShimmierSkill()
        : base(
            LevelConstants.ShimmierSkillId,
            LevelConstants.ShimmierSkillName,
            false)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            var simulationLemming = LemmingManager.SimulateLemming(lemming, true);

            if (simulationLemming.CurrentAction != SliderAction.Instance &&
                (simulationLemming.CurrentAction != FallerAction.Instance ||
                 simulationLemming.FacingDirection != lemming.FacingDirection.GetOpposite()))
                return false;

            var simulationOrientation = simulationLemming.Orientation;
            var simulationPosition = simulationLemming.LevelPosition;

        var gadgetTestRegion = new LevelPositionPair(
            simulationPosition,
            simulationOrientation.MoveUp(simulationPosition, 9));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

            return LemmingAction.PositionIsSolidToLemming(gadgetsNearRegion, simulationLemming, simulationOrientation.MoveUp(simulationPosition, 9)) ||
                   LemmingAction.PositionIsSolidToLemming(gadgetsNearRegion, simulationLemming, simulationOrientation.MoveUp(simulationPosition, 8));
        }

        if (lemming.CurrentAction == SliderAction.Instance ||
            lemming.CurrentAction == DehoisterAction.Instance)
        {
            var oldAction = lemming.CurrentAction;

            var simulationLemming = LemmingManager.SimulateLemming(lemming, true);

            return simulationLemming.CurrentAction != oldAction &&
                   simulationLemming.FacingDirection == lemming.FacingDirection &&
                   (oldAction != DehoisterAction.Instance || simulationLemming.CurrentAction != SliderAction.Instance);
        }

        if (lemming.CurrentAction != JumperAction.Instance)
            return ActionIsAssignable(lemming);

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var gadgetTestRegion1 = new LevelPositionPair(
            lemmingPosition,
            orientation.MoveUp(lemmingPosition, 12));
        var gadgetsNearRegion1 = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion1);

        for (var i = -1; i < 4; i++)
        {
            if (LemmingAction.PositionIsSolidToLemming(gadgetsNearRegion1, lemming, orientation.MoveUp(lemmingPosition, 9 + i)) &&
                !LemmingAction.PositionIsSolidToLemming(gadgetsNearRegion1, lemming, orientation.MoveUp(lemmingPosition, 8 + i)))
                return true;
        }

        return ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        LemmingAction nextAction = lemming.CurrentAction == ClimberAction.Instance ||
                                   lemming.CurrentAction == SliderAction.Instance ||
                                   lemming.CurrentAction == JumperAction.Instance ||
                                   lemming.CurrentAction == DehoisterAction.Instance
            ? ShimmierAction.Instance
            : ReacherAction.Instance;

        nextAction.TransitionLemmingToAction(lemming, false);
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