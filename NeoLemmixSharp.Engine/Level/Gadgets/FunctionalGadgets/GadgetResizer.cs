using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetResizer : GadgetBase, ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private readonly IResizeableGadget[] _gadgets;

    private bool _active = true;
    private int _tickCount;

    public override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetResizer(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        string inputName,
        IResizeableGadget[] gadgets,
        int tickDelay,
        int dw,
        int dh)
        : base(id, orientation, gadgetBounds, 1)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dw = dw;
        _dh = dh;

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

    private sealed class GadgetResizerInput : GadgetInput
    {
        private readonly GadgetResizer _resizer;

        public GadgetResizerInput(string inputName, GadgetResizer resizer)
            : base(inputName)
        {
            _resizer = resizer;
        }

        public override void OnRegistered()
        {
            _resizer._active = false;
        }

        public override void ReactToSignal(bool signal)
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
