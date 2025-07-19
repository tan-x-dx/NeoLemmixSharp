namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GadgetInternalBehaviours;

public sealed class StateTransitionBehaviour : GadgetInternalBehaviour
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

    public override void PerformBehaviour()
    {
        var gadget = LevelScreen.GadgetManager.AllItems[_gadgetId];
        gadget.SetNextState(_stateIndex);
    }
}
