using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShimmierAction : LemmingAction
{
    public static readonly ShimmierAction Instance = new();

    private ShimmierAction()
    {
    }

    public override int Id => LevelConstants.ShimmierActionId;
    public override string LemmingActionName => "shimmier";
    public override int NumberOfAnimationFrames => LevelConstants.ShimmierAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = LevelConstants.TerrainManager;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
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

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

            if (terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            if (terrainManager.PositionOutOfBounds(lemmingPosition))
            {
                LevelConstants.LemmingManager.RemoveLemming(lemming);
                return true;
            }
        }

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9)))
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);

        var checkPosition = orientation.MoveUp(lemmingPosition, 5);
        if (!terrainManager.PixelIsSolidToLemming(lemming, checkPosition))
            return true;

        lemmingPosition = checkPosition;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => -2;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var terrainManager = LevelConstants.TerrainManager;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
            lemmingPosition = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);

            if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            }
        }
        else if (lemming.CurrentAction == SliderAction.Instance ||
                 lemming.CurrentAction == DehoisterAction.Instance)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 2);
            if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
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
                }
            }
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}