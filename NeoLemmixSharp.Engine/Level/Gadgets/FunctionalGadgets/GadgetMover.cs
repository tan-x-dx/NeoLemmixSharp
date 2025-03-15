using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetMover : GadgetBase
{
    private readonly AnimationController _inactiveAnimationController;
    private readonly AnimationController _activeAnimationController;
    private readonly IMoveableGadget[] _gadgets;

    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private bool _active = true;
    private int _tickCount;

    public GadgetMover(
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        string inputName,
        IMoveableGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy)
        : base(1)
    {
        _inactiveAnimationController = inactiveAnimationController;
        _activeAnimationController = activeAnimationController;
        CurrentAnimationController = _inactiveAnimationController;

        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dx = dx;
        _dy = dy;

        RegisterInput(new GadgetMoverInput(inputName, this));
    }

    public override void Tick()
    {
        CurrentAnimationController.Tick();

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

    private sealed class GadgetMoverInput : GadgetInput
    {
        private readonly GadgetMover _gadget;

        public GadgetMoverInput(string inputName, GadgetMover gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void OnRegistered()
        {
            _gadget._active = false;
            _gadget.CurrentAnimationController = _gadget._inactiveAnimationController;
        }

        public override void ReactToSignal(bool signal)
        {
            _gadget._active = signal;

            _gadget.CurrentAnimationController = signal
                ? _gadget._activeAnimationController
                : _gadget._inactiveAnimationController;
        }
    }
}
