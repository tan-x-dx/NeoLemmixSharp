using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class StateTransitionAction : IGadgetAction
{
    private readonly int _gadgetId;
    private readonly int _stateIndex;
    public GadgetActionType ActionType => GadgetActionType.SetGadgetState;

    public StateTransitionAction(
        int gadgetId,
        int stateIndex)
    {
        _gadgetId = gadgetId;
        _stateIndex = stateIndex;
    }

    public void PerformAction(Lemming lemming)
    {
        var gadget = LevelScreen.GadgetManager.AllItems[_gadgetId] as HitBoxGadget ?? throw new Exception("Cannot change state of gadget!");
        gadget.SetNextState(_stateIndex);
    }
}