using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ExiterAction : LemmingAction
{
    public static readonly ExiterAction Instance = new();

    private ExiterAction()
        : base(
            EngineConstants.ExiterActionId,
            EngineConstants.ExiterActionName,
            EngineConstants.ExiterActionSpriteFileName,
            EngineConstants.ExiterAnimationFrames,
            EngineConstants.MaxExiterPhysicsFrames,
            EngineConstants.NoPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (LevelScreen.LevelTimer.OutOfTime)
        {
            lemming.AnimationFrame--;
            lemming.PhysicsFrame--;

            //if UserSetNuking and (L.LemExplosionTimer <= 0) and (Index_LemmingToBeNuked > L.LemIndex) then
            //  Transition(L, baOhnoing);

            return false;
        }

        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.Exit);
        }

        return false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}