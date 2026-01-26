using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;

public sealed class ConstrainedResizeHitBoxGadgetBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper _tickCount;
    private readonly GadgetIdentifier _gadgetIdentifier;

    private readonly int _tickDelay;
    private readonly int _delta;
    private readonly int _max;

    public ConstrainedResizeHitBoxGadgetBehaviour(
        ref nint dataHandle,
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        int delta,
        int max)
        : base(GadgetBehaviourType.GadgetConstrainedResize)
    {
        _tickCount = PointerDataHelper.CreateItem<PointerWrapper>(ref dataHandle);
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
        _max = max;
    }

    protected override void PerformInternalBehaviour()
    {
        if (_tickCount.IntValue < _tickDelay)
        {
            _tickCount.IntValue++;
            return;
        }

        _tickCount.IntValue = 0;

        var gadget = (HitBoxGadget)LevelScreen.GadgetManager.GetGadget(_gadgetIdentifier.GadgetId);

        var dht = new DihedralTransformation(gadget.Orientation, gadget.FacingDirection);

        var p = new Point(0, _delta);


        //  gadget.Resize(_dw, _dh);
    }
}
