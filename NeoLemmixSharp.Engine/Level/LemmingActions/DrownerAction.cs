using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DrownerAction : LemmingAction
{
    public static readonly DrownerAction Instance = new();

    private DrownerAction()
        : base(
            EngineConstants.DrownerActionId,
            EngineConstants.DrownerActionName,
            EngineConstants.DrownerActionSpriteFileName,
            EngineConstants.DrownerAnimationFrames,
            EngineConstants.MaxDrownerPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        /*   var gadgetManager = LevelScreen.GadgetManager;
           Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
           if (!gadgetManager.HasGadgetWithBehaviourAtLemmingPosition(scratchSpaceSpan, lemming, WaterGadgetBehaviour.Instance))
           {
               WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

               return true;
           }*/

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