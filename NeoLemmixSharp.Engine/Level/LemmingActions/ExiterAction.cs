using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ExiterAction : LemmingAction
{
    public static readonly ExiterAction Instance = new();

    private ExiterAction()
        : base(
            LemmingActionConstants.ExiterActionId,
            LemmingActionConstants.ExiterActionName,
            LemmingActionConstants.ExiterActionSpriteFileName,
            LemmingActionConstants.ExiterAnimationFrames,
            LemmingActionConstants.MaxExiterPhysicsFrames,
            EngineConstants.NoPriority,
            LemmingActionBounds.StandardLemmingBounds)
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

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}
