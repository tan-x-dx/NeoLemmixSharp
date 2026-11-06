using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Rewind;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

public sealed class MoveGadgetBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper _tickCount;
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly uint _tickDelay;
    private readonly Point _delta;
    private readonly Point _limitPoint;

    public MoveGadgetBehaviour(
        DataHandle dataHandle,
        GadgetIdentifier gadgetIdentifier,
        uint tickDelay,
        Point delta,
        Point limitPoint)
        : base(GadgetBehaviourType.GadgetMove)
    {
        _tickCount = dataHandle.CreateItem<PointerWrapper>();
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
        _limitPoint = limitPoint;
    }

    protected override void PerformInternalBehaviour()
    {
        if (_tickCount.UintValue < _tickDelay)
        {
            _tickCount.UintValue++;
            return;
        }

        _tickCount.UintValue = 0;

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
