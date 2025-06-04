using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetMover : FunctionalGadget<GadgetMover.GadgetMoverInput>
{
    private readonly IMoveableGadget[] _gadgets;

    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private bool _active = true;
    private int _tickCount;

    public GadgetMover(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        GadgetInputName inputName,
        IMoveableGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy)
        : base(gadgetName, states, initialStateIndex, 1)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dx = dx;
        _dy = dy;

        RegisterInput(new GadgetMoverInput(inputName, this));
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
            _gadgets[i].Move(_dx, _dy);
        }
    }

    protected override void OnChangeStates() { }

    public sealed class GadgetMoverInput : GadgetInput
    {
        private readonly GadgetMover _gadget;

        public GadgetMoverInput(GadgetInputName inputName, GadgetMover gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void OnRegistered()
        {
            _gadget._active = false;
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
