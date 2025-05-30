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
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
        if (lemming.PhysicsFrame == 1 && lemming.IsStartingAction)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            return true;
        }

        if (lemming.PhysicsFrame <= 4)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
        }

        return true;
    }

    protected override RectangularRegion ActionBounds() => LemmingActionBounds.HoisterActionBounds;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var previouslyStartingAction = lemming.IsStartingAction;
        DoMainTransitionActions(lemming, turnAround);
        lemming.IsStartingAction = previouslyStartingAction; // It needs to know what the Climber's value was
    }
}