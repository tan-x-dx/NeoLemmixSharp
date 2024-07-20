using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DrownerAction : LemmingAction
{
    public static readonly DrownerAction Instance = new();

    private DrownerAction()
        : base(
            LevelConstants.DrownerActionId,
            LevelConstants.DrownerActionName,
            LevelConstants.DrownerActionSpriteFileName,
            LevelConstants.DrownerAnimationFrames,
            LevelConstants.MaxDrownerPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!LevelScreen.GadgetManager.HasGadgetWithBehaviourAtLemmingPosition(lemming, WaterGadgetBehaviour.Instance))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (lemming.EndOfAnimation)
        {
            // remove lemming
        }

        return false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => Math.Max(4, 9 - animationFrame);

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}