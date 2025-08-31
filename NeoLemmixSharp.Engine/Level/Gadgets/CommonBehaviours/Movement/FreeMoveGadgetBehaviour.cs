using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

public sealed class FreeMoveGadgetBehaviour : GadgetBehaviour
{
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly int _tickDelay;
    private readonly Point _delta;

    private int _tickCount;

    public FreeMoveGadgetBehaviour(
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        Point delta)
        : base(GadgetBehaviourType.GadgetFreeMove)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
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
        gadget.Move(_delta);
    }
}
