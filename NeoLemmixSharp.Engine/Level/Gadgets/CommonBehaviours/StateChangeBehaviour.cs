using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

public sealed class StateChangeBehaviour : GadgetBehaviour
{
    private readonly int _intendedGadgetStateIndex;

    public StateChangeBehaviour(int intendedGadgetStateIndex)
        : base(GadgetBehaviourType.GadgetChangeInternalState)
    {
        _intendedGadgetStateIndex = intendedGadgetStateIndex;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        ParentGadget.SetState(_intendedGadgetStateIndex);
    }
}
