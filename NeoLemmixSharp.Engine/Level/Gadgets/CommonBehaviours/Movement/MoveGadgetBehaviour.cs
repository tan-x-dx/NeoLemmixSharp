using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

public sealed class MoveGadgetBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper _tickCount;
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly int _tickDelay;
    private readonly Point _delta;
    private readonly Point _limitPoint;

    private ref int TickCountRef => ref _tickCount.IntValue;

    public MoveGadgetBehaviour(
        nint dataHandle,
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        Point delta,
        Point limitPoint)
        : base(GadgetBehaviourType.GadgetMove)
    {
        _tickCount = new PointerWrapper(dataHandle);
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
        _limitPoint = limitPoint;
    }

    protected override void PerformInternalBehaviour(int _)
    {
        if (TickCountRef < _tickDelay)
        {
            TickCountRef++;
            return;
        }

        TickCountRef = 0;

        var gadget = (IMoveableGadget)LevelScreen.GadgetManager.GetGadget(_gadgetIdentifier.GadgetId);
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
