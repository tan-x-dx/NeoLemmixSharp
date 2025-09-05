using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShimmierAction : LemmingAction
{
    public static readonly ShimmierAction Instance = new();

    private ShimmierAction()
        : base(
            LemmingActionConstants.ShimmierActionId,
            LemmingActionConstants.ShimmierActionName,
            LemmingActionConstants.ShimmierActionSpriteFileName,
            LemmingActionConstants.ShimmierAnimationFrames,
            LemmingActionConstants.MaxShimmierPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.ShimmierActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if ((lemming.PhysicsFrame & 1) != 0)
            return true;

        var i = 0;
        // Check whether we find terrain to walk onto
        for (; i < 3; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, i)) &&
                !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, i + 1)))
            {
                lemmingPosition = orientation.Move(lemmingPosition, dx, i);
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }
        }

        // Check whether we find terrain to hoist onto
        for (; i < 6; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, i)) &&
                !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, i + 1)))
            {
                lemmingPosition = orientation.Move(lemmingPosition, dx, i - 4);
                lemming.IsStartingAction = false;
                HoisterAction.Instance.TransitionLemmingToAction(lemming, false);
                lemming.PhysicsFrame += 2;
                lemming.AnimationFrame += 2;
                return true;
            }
        }

        // Check whether we fall down due to a wall
        for (; i < 8; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, i)))
            {
                if (lemming.State.IsSlider)
                {
                    SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                }
                else
                {
                    FallerAction.Instance.TransitionLemmingToAction(lemming, false);
                }

                return true;
            }
        }

        var pixel9AboveIsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 9));
        // Check whether we fall down due to not enough ceiling terrain
        if (!pixel9AboveIsSolid &&
            !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 10)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Check whether we fall down due a checkerboard ceiling
        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 8)) &&
            !pixel9AboveIsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Move along
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            if (LevelScreen.TerrainManager.PositionOutOfBounds(lemmingPosition))
            {
                LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathVoid);
                return true;
            }
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 9)))
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);

        var checkPosition = orientation.MoveUp(lemmingPosition, 5);
        if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, checkPosition))
            return true;

        lemmingPosition = checkPosition;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoShimmierTransitionActions(lemming, turnAround);

        DoMainTransitionActions(lemming, turnAround);
    }

    [SkipLocalsInit]
    private static void DoShimmierTransitionActions(Lemming lemming, bool turnAround)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new RectangularRegion(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx, 12));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearLemming);

        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            lemming.FacingDirection = lemming.FacingDirection.GetOpposite();
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            }
        }
        else if (lemming.CurrentAction == SliderAction.Instance ||
                 lemming.CurrentAction == DehoisterAction.Instance)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 2);
            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            }
        }
        else if (lemming.CurrentAction == JumperAction.Instance)
        {
            for (var i = -1; i < 4; i++)
            {
                if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 9 + i)) &&
                    !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 8 + i)))
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, i);
                }
            }
        }
    }
}
