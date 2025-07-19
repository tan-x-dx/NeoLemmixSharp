using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GadgetInternalBehaviours;

public sealed class StateTransitionBehaviour : GeneralBehaviour
{
    private readonly int _gadgetId;
    private readonly int _stateIndex;

    public StateTransitionBehaviour(
        int gadgetId,
        int stateIndex)
    {
        _gadgetId = gadgetId;
        _stateIndex = stateIndex;
    }

    protected override void PerformInternalBehaviour()
    {
        var gadget = LevelScreen.GadgetManager.AllItems[_gadgetId];
        gadget.SetNextState(_stateIndex);
    }
}
