using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetMover : GadgetBase, ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;


    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private readonly HitBoxGadget[] _gadgets;

    private bool _active = true;
    private int _tickCount;
    public override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetMover(
        int id,
        Orientation orientation,
        string inputName,
        HitBoxGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy) : base(id, orientation)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dx = dx;
        _dy = dy;

        RegisterInput(new GadgetMoverInput(inputName, this));
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
            gadget.HitBox.HitBoxRegion.Move(_dx, _dy);
        }
    }

    private sealed class GadgetMoverInput : IGadgetInput
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

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}
