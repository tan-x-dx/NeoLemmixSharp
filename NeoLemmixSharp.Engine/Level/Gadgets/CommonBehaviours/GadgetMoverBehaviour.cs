using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

public sealed class GadgetMoverBehaviour : GadgetBehaviour
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
        : base(GadgetBehaviourType.GadgetMoveFree)
    {
        _gadget = gadget;
        _tickDelay = tickDelay;
        _dx = dx;
        _dy = dy;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
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
