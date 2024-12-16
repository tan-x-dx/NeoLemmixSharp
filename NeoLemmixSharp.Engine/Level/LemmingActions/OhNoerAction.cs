using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class OhNoerAction : LemmingAction
{
    public static readonly OhNoerAction Instance = new();

    private OhNoerAction()
        : base(
            EngineConstants.OhNoerActionId,
            EngineConstants.OhNoerActionName,
            EngineConstants.OhNoerActionSpriteFileName,
            EngineConstants.OhNoerAnimationFrames,
            EngineConstants.MaxOhNoerPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.LevelPosition;

        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.DeregisterBlocker(lemming);
            var nextAction = lemming.CountDownAction;
            nextAction.TransitionLemmingToAction(lemming, false);
            lemming.ClearCountDownAction();
            return !nextAction.IsOneTimeAction();
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
            return true;

        LevelScreen.LemmingManager.DeregisterBlocker(lemming);

        var updraftFallDelta = GetUpdraftFallDelta(lemming, in gadgetsNearLemming);

        var lemmingOrientation = lemming.Orientation;
        lemmingPosition = lemmingOrientation.MoveDown(lemmingPosition, EngineConstants.DefaultFallStep + updraftFallDelta.Y);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => animationFrame < 7 ? 10 : 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    public static void HandleCountDownTransition(Lemming lemming)
    {
        var currentAction = lemming.CurrentAction;

        if (currentAction == NoneAction.Instance)
            return;

        if (currentAction.IsAirborneAction())
        {
            // If in the air, do the action immediately
            lemming.CountDownAction.TransitionLemmingToAction(lemming, false);
            lemming.ClearCountDownAction();
            return;
        }

        Instance.TransitionLemmingToAction(lemming, false); // Otherwise start oh-noing!
    }
}