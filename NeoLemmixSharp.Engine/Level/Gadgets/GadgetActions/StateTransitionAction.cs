using NeoLemmixSharp.Engine.Level.Gadgets.States;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class StateTransitionAction : IGadgetAction
{
    private readonly StatefulGadget _gadget;

    public StateTransitionAction(StatefulGadget gadget)
    {
        _gadget = gadget;
    }

    public void PerformAction(int newState)
    {
        _gadget.SetState(newState);
    }
}