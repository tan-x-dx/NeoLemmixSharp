using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class StateTransitionAction : GadgetAction
{
    private readonly int _gadgetId;
    private readonly int _stateIndex;

    public StateTransitionAction(
        int gadgetId,
        int stateIndex)
        : base(GadgetActionType.SetGadgetState)
    {
        _gadgetId = gadgetId;
        _stateIndex = stateIndex;
    }

    public override void PerformAction(Lemming lemming)
    {
        var gadget = LevelScreen.GadgetManager.AllItems[_gadgetId] as HitBoxGadget ?? throw new Exception("Cannot change state of gadget!");
        gadget.SetNextState(_stateIndex);
    }
}
