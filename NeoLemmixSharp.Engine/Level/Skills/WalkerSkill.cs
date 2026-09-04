using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WalkerSkill : LemmingSkill
{
    public static readonly WalkerSkill Instance = new();

    private WalkerSkill()
        : base(
            LemmingSkillType.WalkerSkill,
            LemmingSkillConstants.WalkerSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        var testUp = orientation.MoveUp(lemmingPosition, 1);
        var testRight = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);

        var gadgetTestRegion = new RectangularRegion(lemmingPosition, testRight);
        LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

        if (lemming.CurrentActionType == LemmingActionType.BuilderAction &&
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testUp) &&
            !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testRight))
        {
            lemmingPosition = testUp;

            WalkerAction.TransitionLemmingToAction(lemming, false);

            return;
        }

        if (lemming.CurrentActionType != LemmingActionType.WalkerAction)
        {
            WalkerAction.TransitionLemmingToAction(lemming, false);

            return;
        }

        // Turn around walking lem, if assigned a walker
        lemming.FacingDirection = lemming.FacingDirection.GetOpposite();

        if (LemmingIsForcedToChangeDirection(in gadgetsNearRegion, lemming))
        {
            // Go one back to cancel the horizontal offset in WalkerAction's update method.
            // unless the Lem will fall down (which is handles already in Transition)
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            {
                lemmingPosition = testRight;
            }
        }

        WalkerAction.TransitionLemmingToAction(lemming, false);
    }

    private static bool LemmingIsForcedToChangeDirection(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming)
    {
        var allBlockers = LevelScreen.LemmingManager.AllBlockers;

        if (allBlockers.Count > 0)
        {
            foreach (var blocker in allBlockers)
            {

            }
        }

        // Special treatment if in one-way-field facing the wrong direction
        // see http://www.lemmingsforums.net/index.php?topic=2640.0
        var facingDirectionAsOrientation = lemming.FacingDirection.ConvertToRelativeOrientation(lemming.Orientation);

        foreach (var gadget in gadgetsNearRegion)
        {
            /*Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.ForceDirection, lemmingPosition, facingDirectionAsOrientation.GetOpposite())*/
        }

        return false;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.BlockerAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.ShimmierAction);
        result.Add(LemmingActionType.LasererAction);
        result.Add(LemmingActionType.ReacherAction);
        result.Add(LemmingActionType.ShruggerAction);

        return result;
    }
}
