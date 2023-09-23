using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShimmierAction : LemmingAction
{
    public static ShimmierAction Instance { get; } = new();

    private ShimmierAction()
    {
    }

    public override int Id => Global.ShimmierActionId;
    public override string LemmingActionName => "shimmier";
    public override int NumberOfAnimationFrames => Global.ShimmierAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => Global.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = Global.TerrainManager;
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if ((lemming.PhysicsFrame & 1) == 0)
        {
            var i = 0;
            // Check whether we find terrain to walk onto
            for (; i < 3; i++)
            {
                if (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, i)) &&
                    !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, i + 1)))
                {
                    lemmingPosition = orientation.Move(lemmingPosition, dx, i);
                    lemming.LevelPosition = lemmingPosition;
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                    return true;
                }
            }

            // Check whether we find terrain to hoist onto
            for (; i < 6; i++)
            {
                if (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, i)) &&
                    !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, i + 1)))
                {
                    lemmingPosition = orientation.Move(lemmingPosition, dx, i - 4);
                    lemming.LevelPosition = lemmingPosition;
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
                if (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, i)))
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
        }

        var pixel9AboveIsSolid = terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 9));
        // Check whether we fall down due to not enough ceiling terrain
        if (!pixel9AboveIsSolid &&
            !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 10)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Check whether we fall down due a checkerboard ceiling
        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 8)) &&
            !pixel9AboveIsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Move along
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;

            if (terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            if (terrainManager.PositionOutOfBounds(lemmingPosition))
            {
                Global.LemmingManager.RemoveLemming(lemming);
                return true;
            }
        }

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9)))
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 5);
        if (terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => -2;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var terrainManager = Global.TerrainManager;
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection());
            lemmingPosition = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);
            lemming.LevelPosition = lemmingPosition;

            if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (lemming.CurrentAction == SliderAction.Instance ||
                 lemming.CurrentAction == DehoisterAction.Instance)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 2);
            lemming.LevelPosition = lemmingPosition;
            if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (lemming.CurrentAction == JumperAction.Instance)
        {
            for (var i = -1; i < 4; i++)
            {
                if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9 + i)) &&
                    !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8 + i)))
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, i);
                    lemming.LevelPosition = lemmingPosition;
                }
            }
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}