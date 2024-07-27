using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SplatterAction : LemmingAction
{
    public static readonly SplatterAction Instance = new();

    private SplatterAction()
        : base(
            LevelConstants.SplatterActionId,
            LevelConstants.SplatterActionName,
            LevelConstants.SplatterActionSpriteFileName,
            LevelConstants.SplatterAnimationFrames,
            LevelConstants.MaxSplatterPhysicsFrames,
            LevelConstants.NoPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathSplat);
        }

        return false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 6;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}