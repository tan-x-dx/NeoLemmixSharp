using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DehoisterAction : LemmingAction
{
    public static readonly DehoisterAction Instance = new();

    private DehoisterAction()
        : base(
            LevelConstants.DehoisterActionId,
            LevelConstants.DehoisterActionName,
            LevelConstants.DehoisterAnimationFrames,
            LevelConstants.MaxDehoisterPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority,
            true,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        if (lemming.EndOfAnimation)
        {
            if (LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 7)))
            {
                SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (lemming.PhysicsFrame < 2)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        var animFrameValue = lemming.PhysicsFrame * 2;

        if (!SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 3))
        {
            if (lemming.CurrentAction == DrownerAction.Instance)
                return false;
        }

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        if (SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 2))
            return true;

        return lemming.CurrentAction != DrownerAction.Instance;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 1;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = lemming.LevelPosition;

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}