using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.LemmingActions;

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
        if (!Gadgets.HasGadgetOfTypeAtPosition(lemming.LevelPosition, GadgetType.Water))
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
}