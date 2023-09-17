using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class GadgetMover : GadgetBase
{
    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private readonly IMoveableGadget[] _gadgets;

    private bool _active;
    private int _tickCount;

    public override GadgetType Type => GadgetType.Mover;
    public override Orientation Orientation => DownOrientation.Instance;

    public IGadgetInput Input { get; }

    public GadgetMover(
        int id,
        RectangularLevelRegion gadgetBounds,
        IMoveableGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy)
        : base(id, gadgetBounds)
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

        for (var i = 0; i < _gadgets.Length; i++)
        {
            var gadget = _gadgets[i];
            gadget.Move(_dx, _dy);
        }
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;
        return null;
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

        public void ReactToSignal(bool signal)
        {
            _mover._active = signal;
        }
    }
}