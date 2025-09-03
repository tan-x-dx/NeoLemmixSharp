using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DrownerAction : LemmingAction
{
    public static readonly DrownerAction Instance = new();

    private DrownerAction()
        : base(
            LemmingActionConstants.DrownerActionId,
            LemmingActionConstants.DrownerActionName,
            LemmingActionConstants.DrownerActionSpriteFileName,
            LemmingActionConstants.DrownerAnimationFrames,
            LemmingActionConstants.MaxDrownerPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.StandardLemmingBounds)
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

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}
