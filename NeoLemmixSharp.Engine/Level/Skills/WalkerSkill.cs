using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WalkerSkill : LemmingSkill
{
    public static readonly WalkerSkill Instance = new();

    private WalkerSkill()
        : base(
            LevelConstants.WalkerSkillId,
            LevelConstants.WalkerSkillName,
            false)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        var testUp = orientation.MoveUp(lemmingPosition, 1);
        var testRight = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            testRight);
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        if (lemming.CurrentAction == BuilderAction.Instance &&
            LemmingAction.PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testUp) &&
            !LemmingAction.PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testRight))
        {
            lemmingPosition = testUp;

            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return;
        }

        if (lemming.CurrentAction != WalkerAction.Instance)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return;
        }

        // Turn around walking lem, if assigned a walker
        lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());

        if (LemmingIsForcedToChangeDirection(in gadgetsNearRegion, lemming))
        {
            // Go one back to cancel the horizontal offset in WalkerAction's update method.
            // unless the Lem will fall down (which is handles already in Transition)
            if (LemmingAction.PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            {
                lemmingPosition = testRight;
            }
        }

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    private static bool LemmingIsForcedToChangeDirection(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming)
    {
        foreach (var blocker in LevelScreen.LemmingManager.AllBlockers)
        {

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

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return BlockerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return BuilderAction.Instance;
        yield return PlatformerAction.Instance;
        yield return StackerAction.Instance;
        yield return ShimmierAction.Instance;
        yield return LasererAction.Instance;
        yield return ReacherAction.Instance;
        yield return ShruggerAction.Instance;
    }
}