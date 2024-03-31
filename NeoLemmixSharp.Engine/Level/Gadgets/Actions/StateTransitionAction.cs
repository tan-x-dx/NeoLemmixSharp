using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class StateTransitionAction : IGadgetAction
{
    private readonly StatefulGadget _gadget;
    private readonly int _stateIndex;

    public StateTransitionAction(
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