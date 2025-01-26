using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class StateTransitionAction : IGadgetAction
{
    private readonly HitBoxGadget _gadget;
    private readonly int _stateIndex;

    public StateTransitionAction(
        HitBoxGadget gadget,
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