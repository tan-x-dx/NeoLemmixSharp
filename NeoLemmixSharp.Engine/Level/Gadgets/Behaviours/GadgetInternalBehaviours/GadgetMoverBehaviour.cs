using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GadgetInternalBehaviours;

public sealed class GadgetMoverBehaviour : GadgetInternalBehaviour
{
    private readonly IMoveableGadget _gadget;
    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private int _tickCount;

    public GadgetMoverBehaviour(
        IMoveableGadget gadget,
        int tickDelay,
        int dx,
        int dy)
    {
        _gadget = gadget;
        _tickDelay = tickDelay;
        _dx = dx;
        _dy = dy;
    }

    public override void PerformBehaviour()
    {
        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        _gadget.Move(_dx, _dy);
    }
}
