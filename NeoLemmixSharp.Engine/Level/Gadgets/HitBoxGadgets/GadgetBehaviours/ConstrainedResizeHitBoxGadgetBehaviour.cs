using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;

public sealed class ConstrainedResizeHitBoxGadgetBehaviour : GadgetBehaviour
{
    private readonly GadgetIdentifier _gadgetIdentifier;

    private readonly int _tickDelay;
    private readonly int _delta;
    private readonly int _max;

    private int _tickCount;

    public ConstrainedResizeHitBoxGadgetBehaviour(
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        int delta,
        int max)
        : base(GadgetBehaviourType.GadgetConstrainedResize)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
        _max = max;
    }

    protected override void PerformInternalBehaviour(int _)
    {
        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        var gadget = (HitBoxGadget)LevelScreen.GadgetManager.GetGadget(_gadgetIdentifier.GadgetId);

        var dht = new DihedralTransformation(gadget.Orientation, gadget.FacingDirection);

        var p = new Point(0, _delta);


      //  gadget.Resize(_dw, _dh);
    }
}
