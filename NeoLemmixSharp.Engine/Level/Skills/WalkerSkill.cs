using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WalkerSkill : LemmingSkill
{
    public static readonly WalkerSkill Instance = new();

    private WalkerSkill()
        : base(
            EngineConstants.WalkerSkillId,
            EngineConstants.WalkerSkillName)
    {
    }

    [SkipLocalsInit]
    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        var testUp = orientation.MoveUp(lemmingPosition, 1);
        var testRight = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new LevelRegion(
            lemmingPosition,
            testRight);
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearRegion);

        if (lemming.CurrentAction == BuilderAction.Instance &&
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testUp) &&
            !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testRight))
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
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            {
                lemmingPosition = testRight;
            }
        }

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
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

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingActionHasher.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(BlockerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(ShimmierAction.Instance);
        result.Add(LasererAction.Instance);
        result.Add(ReacherAction.Instance);
        result.Add(ShruggerAction.Instance);

        return result;
    }
}