using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetBehaviours;

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