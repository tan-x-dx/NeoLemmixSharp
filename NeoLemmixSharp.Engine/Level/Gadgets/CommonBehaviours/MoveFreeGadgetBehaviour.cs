using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

public sealed class MoveFreeGadgetBehaviour : GadgetBehaviour
{
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private int _tickCount;

    public MoveFreeGadgetBehaviour(
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        int dx,
        int dy)
        : base(GadgetBehaviourType.GadgetMoveFree)
    {
        _gadgetIdentifier = gadgetIdentifier;
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

        var gadget = (IMoveableGadget)LevelScreen.GadgetManager.AllItems[_gadgetIdentifier.GadgetId];
        gadget.Move(_dx, _dy);
    }
}
