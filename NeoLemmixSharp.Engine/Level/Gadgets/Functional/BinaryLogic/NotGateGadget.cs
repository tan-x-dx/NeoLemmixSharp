using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class NotGateGadget : GadgetBase, ILogicGateGadget
{
    private bool _shouldTick;

    public override GadgetBehaviour GadgetBehaviour => LogicGateGadgetBehaviour.Instance;
    public override Orientation Orientation => DownOrientation.Instance;

    public LogicGateGadgetInput Input { get; }
    public GadgetOutput Output { get; } = new();

    public NotGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        IControlledAnimationGadgetRenderer? renderer,
        LogicGateGadgetInput input)
        : base(id, gadgetBounds, renderer)
    {
        Input = input;

        Input.SetLogicGate(this);
    }

    public override void Tick()
    {
        if (_shouldTick)
            return;

        _shouldTick = false;
        Output.SetSignal(!Input.Signal);
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;

        return null;
    }

    public void EvaluateInputs()
    {
        _shouldTick = true;
    }
}