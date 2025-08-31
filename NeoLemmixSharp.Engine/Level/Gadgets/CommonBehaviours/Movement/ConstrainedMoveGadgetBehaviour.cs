using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

public sealed class ConstrainedMoveGadgetBehaviour : GadgetBehaviour
{
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly int _tickDelay;
    private readonly Point _delta;
    private readonly Point _limitPoint;

    private int _tickCount;

    public ConstrainedMoveGadgetBehaviour(
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        Point delta,
        Point limitPoint)
        : base(GadgetBehaviourType.GadgetConstrainedMove)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
        _limitPoint = limitPoint;
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
        var constrainedDelta = GetConstrainedDelta(gadget.Position);
        gadget.Move(constrainedDelta);
    }

    private Point GetConstrainedDelta(Point gadgetPosition)
    {
        var dx = GetConstrainedDelta(_delta.X, _limitPoint.X, gadgetPosition.X);
        var dy = GetConstrainedDelta(_delta.Y, _limitPoint.Y, gadgetPosition.Y);
        return new Point(dx, dy);
    }

    private static int GetConstrainedDelta(int delta, int limit, int gadgetPosition)
    {
        var x = limit - gadgetPosition;

        if (delta > 0)
            return Math.Min(delta, x);

        if (delta < 0)
            return Math.Max(delta, x);

        return 0;
    }
}
