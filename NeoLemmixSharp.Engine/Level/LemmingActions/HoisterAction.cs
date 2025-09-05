using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class HoisterAction : LemmingAction
{
    public static readonly HoisterAction Instance = new();

    private HoisterAction()
        : base(
            LemmingActionConstants.HoisterActionId,
            LemmingActionConstants.HoisterActionName,
            LemmingActionConstants.HoisterActionSpriteFileName,
            LemmingActionConstants.HoisterAnimationFrames,
            LemmingActionConstants.MaxHoisterPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.HoisterActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Data.Orientation;
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;

        if (lemming.Data.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
        if (lemming.Data.PhysicsFrame == 1 && lemming.Data.IsStartingAction)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            return true;
        }

        if (lemming.Data.PhysicsFrame <= 4)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var previouslyStartingAction = lemming.Data.IsStartingAction;
        DoMainTransitionActions(lemming, turnAround);
        lemming.Data.IsStartingAction = previouslyStartingAction; // It needs to know what the Climber's value was
    }
}
