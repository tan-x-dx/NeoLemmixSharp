using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class LogicGateGadgetInput : IGadgetInput
{
    private ILogicGateGadget _gadget;

    public string InputName { get; }
    public bool Signal { get; private set; }

    public LogicGateGadgetInput(string inputName)
    {
        InputName = inputName;
    }

    public void SetLogicGate(ILogicGateGadget gadget)
    {
        _gadget = gadget;
    }

    public void OnRegistered()
    {
    }

    public void ReactToSignal(bool signal)
    {
        Signal = signal;
        _gadget.EvaluateInputs();
    }
}