using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class WalkerAction : LemmingAction
{
    public static readonly WalkerAction Instance = new();

    private WalkerAction()
        : base(
            LemmingActionConstants.WalkerActionId,
            LemmingActionConstants.WalkerActionName,
            LemmingActionConstants.WalkerActionSpriteFileName,
            LemmingActionConstants.WalkerAnimationFrames,
            LemmingActionConstants.MaxWalkerPhysicsFrames,
            EngineConstants.WalkerMovementPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        var dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (dy < 0 &&
            lemming.State.IsSlider &&
            DehoisterAction.LemmingCanDehoist(lemming, true, in gadgetsNearLemming))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        if (dy > EngineConstants.MaxStepUp)
        {
            if (lemming.State.IsClimber)
            {
                ClimberAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            }
        }
        else if (dy > 2)
        {
            AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
        }
        else if (dy >= 0)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, dy);
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (dy < -3)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 4);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (dy >= 0)
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, dy);

        return true;
    }

    [SkipLocalsInit]
    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        gadgetManager.GetAllGadgetsNearPosition(scratchSpaceSpan, lemming.AnchorPosition, out var gadgetsNearRegion);

        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemming.AnchorPosition))
        {
            DoMainTransitionActions(lemming, turnAround);
            return;
        }

        FallerAction.Instance.TransitionLemmingToAction(lemming, turnAround);
    }
}
