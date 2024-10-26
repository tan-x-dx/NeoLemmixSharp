using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class GadgetMover : GadgetBase, IReactiveGadget
{
    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private readonly IMoveableGadget[] _gadgets;

    private bool _active = true;
    private int _tickCount;

    public override GadgetBehaviour GadgetBehaviour => FunctionalGadgetBehaviour.Instance;
    public override Orientation Orientation => DownOrientation.Instance;

    public GadgetMoverInput Input { get; }

    public GadgetMover(
        int id,
        RectangularHitBoxRegion gadgetBounds,
        IControlledAnimationGadgetRenderer? renderer,
        IMoveableGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy)
        : base(id, gadgetBounds, renderer)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dx = dx;
        _dy = dy;

        Input = new GadgetMoverInput("Input", this);
    }

    public override void Tick()
    {
        if (!_active)
            return;

        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        foreach (var gadget in _gadgets.AsSpan())
        {
            gadget.Move(_dx, _dy);
        }
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;
        return null;
    }

    public sealed class GadgetMoverInput : IGadgetInput
    {
        private readonly GadgetMover _mover;
        public string InputName { get; }

        public GadgetMoverInput(string inputName, GadgetMover mover)
        {
            InputName = inputName;
            _mover = mover;
        }

        public void OnRegistered()
        {
            _mover._active = false;
        }

        public void ReactToSignal(bool signal)
        {
            _mover._active = signal;
        }
    }
}