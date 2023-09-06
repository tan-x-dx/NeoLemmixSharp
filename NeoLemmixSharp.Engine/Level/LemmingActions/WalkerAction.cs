using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class WalkerAction : LemmingAction
{
    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public override int Id => GameConstants.WalkerActionId;
    public override string LemmingActionName => "walker";
    public override int NumberOfAnimationFrames => GameConstants.WalkerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.WalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var lemmingPosition = lemming.LevelPosition;

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;
        var dy = FindGroundPixel(lemming, lemmingPosition);

        if (dy > 0 &&
            lemming.State.IsSlider &&
            LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        if (dy < -6)
        {
            if (lemming.State.IsClimber)
            {
                ClimberAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection());
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (dy < -2)
        {
            AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
            lemming.LevelPosition = lemmingPosition;
        }
        else if (dy < 1)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, dy);
            lemming.LevelPosition = lemmingPosition;
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming, lemmingPosition);

        if (dy > 3)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 4);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (dy <= 0)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, dy);
        lemming.LevelPosition = lemmingPosition;

        return true;
    }

    protected override int TopLeftBoundsDeltaX() => -2;
    protected override int TopLeftBoundsDeltaY() => 10;

    protected override int BottomRightBoundsDeltaX() => 3;

    public static bool LemmingCanDehoist(Lemming lemming, bool alreadyMoved)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        LevelPosition currentPosition;
        LevelPosition nextPosition;
        if (alreadyMoved)
        {
            nextPosition = lemming.LevelPosition;
            currentPosition = orientation.MoveLeft(nextPosition, dx);
        }
        else
        {
            currentPosition = lemming.LevelPosition;
            nextPosition = orientation.MoveRight(currentPosition, dx);
        }

        if (Terrain.PositionOutOfBounds(nextPosition) ||
            (!Terrain.PixelIsSolidToLemming(lemming, currentPosition) ||
             Terrain.PixelIsSolidToLemming(lemming, nextPosition)))
            return false;

        for (var i = 1; i < 4; i++)
        {
            if (Terrain.PixelIsSolidToLemming(lemming, orientation.MoveDown(nextPosition, i)))
                return false;
            if (!Terrain.PixelIsSolidToLemming(lemming, orientation.MoveDown(currentPosition, i)))
                return true;
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (Terrain.PixelIsSolidToLemming(lemming, lemming.LevelPosition))
        {
            base.TransitionLemmingToAction(lemming, turnAround);
            return;
        }

        FallerAction.Instance.TransitionLemmingToAction(lemming, turnAround);
    }
}