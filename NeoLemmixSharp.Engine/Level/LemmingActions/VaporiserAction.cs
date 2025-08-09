using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class VaporiserAction : LemmingAction
{
    public static readonly VaporiserAction Instance = new();

    private VaporiserAction()
        : base(
            LemmingActionConstants.VaporiserActionId,
            LemmingActionConstants.VaporiserActionName,
            LemmingActionConstants.VaporiserActionSpriteFileName,
            LemmingActionConstants.VaporiserAnimationFrames,
            LemmingActionConstants.MaxVaporizerPhysicsFrames,
            EngineConstants.NoPriority,
            LemmingActionBounds.VaporiserActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathFire);
        }

        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}
