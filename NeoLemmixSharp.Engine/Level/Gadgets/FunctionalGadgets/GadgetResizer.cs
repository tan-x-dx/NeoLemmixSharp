using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetResizer : GadgetBase, ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private readonly HitBoxGadget[] _gadgets;

    private bool _active = true;
    private int _tickCount;

    public override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetResizer(
        int id,
        Orientation orientation,
        string inputName,
        HitBoxGadget[] gadgets,
        int tickDelay,
        int dw,
        int dh) : base(id, orientation)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dw = dw;
        _dh = dh;

        for (var i = 0; i < _gadgets.Length; i++)
        {
            if (!_gadgets[i].IsResizable)
                throw new InvalidOperationException("Gadget cannot be resized!");
        }

        RegisterInput(new GadgetResizerInput(inputName, this));
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

        for (var i = 0; i < _gadgets.Length; i++)
        {
            _gadgets[i].Resize(_dw, _dh);
        }
    }

    private sealed class GadgetResizerInput : IGadgetInput
    {
        private readonly GadgetResizer _resizer;
        public string InputName { get; }

        public GadgetResizerInput(string inputName, GadgetResizer resizer)
        {
            InputName = inputName;
            _resizer = resizer;
        }

        public void OnRegistered()
        {
            _resizer._active = false;
        }

        public void ReactToSignal(bool signal)
        {
            _resizer._active = signal;
        }
    }

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}
