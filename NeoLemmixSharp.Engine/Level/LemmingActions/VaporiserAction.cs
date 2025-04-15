using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class VaporiserAction : LemmingAction
{
    public static readonly VaporiserAction Instance = new();

    private VaporiserAction()
        : base(
            EngineConstants.VaporiserActionId,
            EngineConstants.VaporiserActionName,
            EngineConstants.VaporiserActionSpriteFileName,
            EngineConstants.VaporiserAnimationFrames,
            EngineConstants.MaxVaporizerPhysicsFrames,
            EngineConstants.NoPriority)
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

    protected override RectangularRegion ActionBounds() => LemmingActionBounds.VaporiserActionBounds;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}