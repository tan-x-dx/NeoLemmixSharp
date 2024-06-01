using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SliderAction : LemmingAction
{
    private const int MaxYCheckOffset = 7;

    public static readonly SliderAction Instance = new();

    private SliderAction()
        : base(
            LevelConstants.SliderActionId,
            LevelConstants.SliderActionName,
            LevelConstants.SliderAnimationFrames,
            LevelConstants.MaxSliderPhysicsFrames,
            LevelConstants.PermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        if (!SliderTerrainChecks(lemming, orientation, MaxYCheckOffset) &&
            lemming.CurrentAction == DrownerAction.Instance)
            return false;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        return SliderTerrainChecks(lemming, orientation, MaxYCheckOffset) ||
               lemming.CurrentAction != DrownerAction.Instance;
    }

    public static bool SliderTerrainChecks(
        Lemming lemming,
        Orientation orientation,
        int maxYOffset)
    {
        ref var lemmingPosition = ref lemming.LevelPosition;
        var lemmingDehoistPosition = lemming.DehoistPin;
        var dx = lemming.FacingDirection.DeltaX;

        var gadgetTestRegion = new LevelPositionPair(
            orientation.Move(lemmingPosition, -dx, -1),
            orientation.Move(lemmingPosition, dx, LevelConstants.MaxStepUp + 1));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        var hasPixelAtLemmingPosition = SliderHasPixelAt(in gadgetsNearRegion, lemmingPosition);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(in gadgetsNearRegion, orientation.MoveUp(lemmingPosition, 1)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!SliderHasPixelAt(in gadgetsNearRegion, orientation.MoveUp(lemmingPosition, Math.Min(maxYOffset, MaxYCheckOffset))))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!hasPixelAtLemmingPosition)
            return true;

        var gadgetSet = LevelScreen.GadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

        foreach (var gadget in gadgetSet)
        {
            if (gadget.GadgetBehaviour != WaterGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                continue;

            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            if (lemming.State.IsSwimmer)
            {
                SwimmerAction.Instance.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_SWIMMING, L.Position); ??
            }
            else
            {
                DrownerAction.Instance.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_DROWNING, L.Position); ??
            }
            //water.OnLemmingInHitBox(lemming);

            return true;
        }

        var leftPos = orientation.MoveLeft(lemmingPosition, dx);
        if (!SliderHasPixelAt(in gadgetsNearRegion, leftPos))
            return true;

        lemmingPosition = leftPos;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
        return false;

        bool SliderHasPixelAt(
            in GadgetSet gadgetsNearRegion1,
            LevelPosition testPosition)
        {
            return PositionIsSolidToLemming(gadgetsNearRegion1, lemming, testPosition) ||
                   (orientation.MatchesHorizontally(testPosition, lemming.LevelPosition) &&
                    orientation.MatchesVertically(testPosition, lemmingDehoistPosition) &&
                    PositionIsSolidToLemming(gadgetsNearRegion1, lemming, orientation.MoveDown(testPosition, 1)));
        }
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -6;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 0;

    public override LevelPosition GetFootPosition(Lemming lemming, LevelPosition anchorPosition)
    {
        return lemming.Orientation.MoveLeft(anchorPosition, lemming.FacingDirection.DeltaX);
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = new LevelPosition(-1, -1);

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}