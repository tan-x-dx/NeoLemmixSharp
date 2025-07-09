using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SliderAction : LemmingAction
{
    private const int MaxYCheckOffset = 7;

    public static readonly SliderAction Instance = new();

    private SliderAction()
        : base(
            LemmingActionConstants.SliderActionId,
            LemmingActionConstants.SliderActionName,
            LemmingActionConstants.SliderActionSpriteFileName,
            LemmingActionConstants.SliderAnimationFrames,
            LemmingActionConstants.MaxSliderPhysicsFrames,
            EngineConstants.PermanentSkillPriority,
            LemmingActionBounds.ClimberActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        if (!SliderTerrainChecks(lemming, orientation, MaxYCheckOffset, in gadgetsNearLemming) &&
            lemming.CurrentAction == DrownerAction.Instance)
            return false;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        return SliderTerrainChecks(lemming, orientation, MaxYCheckOffset, in gadgetsNearLemming) ||
               lemming.CurrentAction != DrownerAction.Instance;
    }

    public static bool SliderTerrainChecks(
        Lemming lemming,
        Orientation orientation,
        int maxYOffset,
        in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var lemmingDehoistPosition = lemming.DehoistPin;
        var dx = lemming.FacingDirection.DeltaX;

        var hasPixelAtLemmingPosition = SliderHasPixelAt(in gadgetsNearLemming, lemmingPosition);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(in gadgetsNearLemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!SliderHasPixelAt(in gadgetsNearLemming, orientation.MoveUp(lemmingPosition, Math.Min(maxYOffset, MaxYCheckOffset))))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!hasPixelAtLemmingPosition)
            return true;

        foreach (var gadget in gadgetsNearLemming)
        {
            /* if (gadget.GadgetBehaviour != WaterGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                 continue;*/

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
        if (!SliderHasPixelAt(in gadgetsNearLemming, leftPos))
            return true;

        lemmingPosition = leftPos;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
        return false;

        bool SliderHasPixelAt(
            in GadgetEnumerable gadgetsNearLemming1,
            Point testPosition)
        {
            return PositionIsSolidToLemming(in gadgetsNearLemming1, lemming, testPosition) ||
                   (orientation.MatchesHorizontally(testPosition, lemming.AnchorPosition) &&
                    orientation.MatchesVertically(testPosition, lemmingDehoistPosition) &&
                    PositionIsSolidToLemming(in gadgetsNearLemming1, lemming, orientation.MoveDown(testPosition, 1)));
        }
    }

    public override Point GetFootPosition(Lemming lemming, Point anchorPosition)
    {
        return lemming.Orientation.MoveLeft(anchorPosition, lemming.FacingDirection.DeltaX);
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = new Point(-1, -1);

        DoMainTransitionActions(lemming, turnAround);
    }
}
