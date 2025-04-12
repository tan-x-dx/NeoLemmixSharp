using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShruggerAction : LemmingAction
{
    public static readonly ShruggerAction Instance = new();

    private ShruggerAction()
        : base(
            EngineConstants.ShruggerActionId,
            EngineConstants.ShruggerActionName,
            EngineConstants.ShruggerActionSpriteFileName,
            EngineConstants.ShruggerAnimationFrames,
            EngineConstants.MaxShruggerPhysicsFrames,
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
}