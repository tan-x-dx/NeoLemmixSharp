using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Rewind;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;

public sealed class ConstrainedResizeHitBoxGadgetBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper _tickCount;
    private readonly GadgetIdentifier _gadgetIdentifier;

    private readonly uint _tickDelay;
    private readonly int _delta;
    private readonly int _max;

    public ConstrainedResizeHitBoxGadgetBehaviour(
        DataHandle dataHandle,
        GadgetIdentifier gadgetIdentifier,
        uint tickDelay,
        int delta,
        int max)
        : base(GadgetBehaviourType.GadgetConstrainedResize)
    {
        _tickCount = dataHandle.CreateItem<PointerWrapper>();
        _gadgetIdentifier = gadgetIdentifier;
        _tickDelay = tickDelay;
        _delta = delta;
        _max = max;
    }

    protected override void PerformInternalBehaviour()
    {
        if (_tickCount.UintValue < _tickDelay)
        {
            _tickCount.UintValue++;
            return;
        }

        _tickCount.UintValue = 0;

        var gadget = (HitBoxGadget)LevelScreen.GadgetManager.GetGadget(_gadgetIdentifier.GadgetId);

        var dht = gadget.DihedralTransformation;

        var p = new Point(0, _delta);


        //  gadget.Resize(_dw, _dh);
    }
}
