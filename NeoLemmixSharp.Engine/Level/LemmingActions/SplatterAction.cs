using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SplatterAction : LemmingAction
{
    public static readonly SplatterAction Instance = new();

    private SplatterAction()
        : base(
            EngineConstants.SplatterActionId,
            EngineConstants.SplatterActionName,
            EngineConstants.SplatterActionSpriteFileName,
            EngineConstants.SplatterAnimationFrames,
            EngineConstants.MaxSplatterPhysicsFrames,
            EngineConstants.NoPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathSplat);
        }

        return false;
    }

    protected override RectangularRegion ActionBounds() => LemmingActionBounds.SplatterActionBounds;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}