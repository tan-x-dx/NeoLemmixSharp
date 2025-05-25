using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetResizer : FunctionalGadget<GadgetResizer.GadgetResizerInput>
{
    private readonly HitBoxGadget[] _gadgets;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private bool _active = true;
    private int _tickCount;

    public GadgetResizer(
        GadgetState state0,
        GadgetState state1,
        bool startActive,
        string inputName,
        HitBoxGadget[] gadgets,
        int tickDelay,
        int dw,
        int dh)
        : base(state0, state1, startActive, 1)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dw = dw;
        _dh = dh;

        RegisterInput(new GadgetResizerInput(inputName, this));
    }

    protected override void OnTick()
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

    protected override void OnChangeStates() { }

    public sealed class GadgetResizerInput : GadgetInput
    {
        private readonly GadgetResizer _gadget;

        public GadgetResizerInput(string inputName, GadgetResizer gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void OnRegistered()
        {
            _gadget._active = false;
            //    _gadget.CurrentAnimationController = _gadget._inactiveAnimationController;
        }

        public override void ReactToSignal(bool signal)
        {
            _gadget._active = signal;

            //    _gadget.CurrentAnimationController = signal
            //        ? _gadget._activeAnimationController
            //        : _gadget._inactiveAnimationController;
        }
    }
}
