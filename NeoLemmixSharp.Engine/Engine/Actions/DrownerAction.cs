using NeoLemmixSharp.Engine.Engine.Gadgets;

namespace NeoLemmixSharp.Engine.Engine.Actions;

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

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!GadgetCollections.Waters.TryGetGadgetThatMatchesTypeAndOrientation(lemming.LevelPosition, lemming.Orientation, out _))
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