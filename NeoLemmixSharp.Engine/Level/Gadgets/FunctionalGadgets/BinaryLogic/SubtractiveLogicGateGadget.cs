using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class SubtractiveLogicGateGadget : GadgetBase
{
    private readonly AnimationController _inactiveAnimationController;
    private readonly AnimationController _activeAnimationController;

    public GadgetOutput Output { get; } = new();
    private bool _shouldTick;

    protected SubtractiveLogicGateGadget(
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        int expectedNumberOfInputs)
        : base(expectedNumberOfInputs)
    {
        _inactiveAnimationController = inactiveAnimationController;
        _activeAnimationController = activeAnimationController;
    }

    public sealed override void Tick()
    {
        CurrentAnimationController.Tick();

        if (!_shouldTick)
            return;

        _shouldTick = false;

        var isActive = EvaluateInputs();
        Output.SetSignal(isActive);

        CurrentAnimationController = isActive
            ? _activeAnimationController
            : _inactiveAnimationController;
    }

    protected abstract bool EvaluateInputs();

    protected sealed class SubtractiveLogicGateGadgetInput : GadgetInput
    {
        private readonly SubtractiveLogicGateGadget _gadget;

        public bool Signal { get; private set; }

        public SubtractiveLogicGateGadgetInput(string inputName, SubtractiveLogicGateGadget gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void ReactToSignal(bool signal)
        {
            Signal = signal;
            _gadget._shouldTick = true;
        }
    }
}

public sealed class NotGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetInput _input;

    public NotGateGadget(
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        string inputName)
        : base(inactiveAnimationController, activeAnimationController, 1)
    {
        _input = new SubtractiveLogicGateGadgetInput(inputName, this);
        RegisterInput(_input);
    }

    protected override bool EvaluateInputs()
    {
        return !_input.Signal;
    }
}

public sealed class XorGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetInput _input1;
    private readonly SubtractiveLogicGateGadgetInput _input2;

    public XorGateGadget(
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        string input1Name,
        string input2Name)
        : base(inactiveAnimationController, activeAnimationController, 2)
    {
        _input1 = new SubtractiveLogicGateGadgetInput(input1Name, this);
        _input2 = new SubtractiveLogicGateGadgetInput(input2Name, this);
        RegisterInput(_input1);
        RegisterInput(_input2);
    }

    protected override bool EvaluateInputs()
    {
        return _input1.Signal ^ _input2.Signal;
    }
}