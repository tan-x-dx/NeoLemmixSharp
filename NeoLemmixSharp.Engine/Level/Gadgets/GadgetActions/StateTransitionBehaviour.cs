using NeoLemmixSharp.Engine.Level.Gadgets.States;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class StateTransitionBehaviour : IGadgetBehaviour
{
    private readonly StatefulGadget _gadget;
    private readonly int _stateIndex;

    public StateTransitionBehaviour(
        StatefulGadget gadget,
        int stateIndex)
    {
        _gadget = gadget;
        _stateIndex = stateIndex;
    }

    public void PerformAction(Lemming lemming)
    {
        _gadget.SetNextState(_stateIndex);
    }
}