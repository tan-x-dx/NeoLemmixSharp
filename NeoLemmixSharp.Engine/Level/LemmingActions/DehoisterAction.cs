using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DehoisterAction : LemmingAction
{
    public static readonly DehoisterAction Instance = new();

    private DehoisterAction()
        : base(
            LemmingActionConstants.DehoisterActionId,
            LemmingActionConstants.DehoisterActionName,
            LemmingActionConstants.DehoisterActionSpriteFileName,
            LemmingActionConstants.DehoisterAnimationFrames,
            LemmingActionConstants.MaxDehoisterPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.HoisterActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Data.Orientation;
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;

        if (lemming.Data.EndOfAnimation)
        {
            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 7)))
            {
                SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (lemming.Data.PhysicsFrame < 2)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        var animFrameValue = lemming.Data.PhysicsFrame * 2;

        if (!SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 3, in gadgetsNearLemming) &&
            lemming.CurrentAction == DrownerAction.Instance)
            return false;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        return SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 2, in gadgetsNearLemming) ||
               lemming.CurrentAction != DrownerAction.Instance;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.Data.DehoistPin = lemming.Data.AnchorPosition;

        DoMainTransitionActions(lemming, turnAround);
    }

    public static bool LemmingCanDehoist(
        Lemming lemming,
        bool alreadyMoved,
        in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Data.Orientation;
        var dx = lemming.Data.FacingDirection.DeltaX;
        Point currentPosition;
        Point nextPosition;
        if (alreadyMoved)
        {
            nextPosition = lemming.Data.AnchorPosition;
            currentPosition = orientation.MoveLeft(nextPosition, dx);
        }
        else
        {
            currentPosition = lemming.Data.AnchorPosition;
            nextPosition = orientation.MoveRight(currentPosition, dx);
        }

        if (LevelScreen.TerrainManager.PositionOutOfBounds(nextPosition) ||
            !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, currentPosition) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, nextPosition))
            return false;

        for (var i = 1; i < 4; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveDown(nextPosition, i)))
                return false;
            if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveDown(currentPosition, i)))
                return true;
        }

        return true;
    }
}
