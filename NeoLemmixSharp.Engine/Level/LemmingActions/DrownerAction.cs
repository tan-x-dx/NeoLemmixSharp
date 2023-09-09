using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DrownerAction : LemmingAction
{
    public static DrownerAction Instance { get; } = new();

    private DrownerAction()
    {
    }

    public override int Id => GameConstants.DrownerActionId;
    public override string LemmingActionName => "drowner";
    public override int NumberOfAnimationFrames => GameConstants.DrownerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => GameConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!GadgetManager.HasGadgetOfTypeAtPosition(lemming.LevelPosition, GadgetType.Water))
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