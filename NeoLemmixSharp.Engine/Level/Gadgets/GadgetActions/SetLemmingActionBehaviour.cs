using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class SetLemmingActionBehaviour : IGadgetBehaviour
{
    private readonly LemmingAction _action;

    public SetLemmingActionBehaviour(LemmingAction action)
    {
        _action = action;
    }

    public void PerformAction(Lemming lemming)
    {
        _action.TransitionLemmingToAction(lemming, false);
    }
}