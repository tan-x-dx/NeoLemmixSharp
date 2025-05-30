using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShruggerAction : LemmingAction
{
    public static readonly ShruggerAction Instance = new();

    private ShruggerAction()
        : base(
            LemmingActionConstants.ShruggerActionId,
            LemmingActionConstants.ShruggerActionName,
            LemmingActionConstants.ShruggerActionSpriteFileName,
            LemmingActionConstants.ShruggerAnimationFrames,
            LemmingActionConstants.MaxShruggerPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}