using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SetLemmingActionAction : IGadgetAction
{
    private readonly LemmingAction _action;
    public GadgetActionType ActionType => GadgetActionType.SetLemmingAction;

    public SetLemmingActionAction(LemmingAction action)
    {
        _action = action;
    }

    public void PerformAction(Lemming lemming)
    {
        _action.TransitionLemmingToAction(lemming, false);
    }
}