using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GadgetInternalBehaviours;

public sealed class GadgetResizer : GeneralBehaviour
{
    private readonly HitBoxGadget _gadget;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private int _tickCount;

    public GadgetResizer(
        HitBoxGadget gadget,
        int tickDelay,
        int dw,
        int dh)
    {
        _tickDelay = tickDelay;
        _gadget = gadget;
        _dw = dw;
        _dh = dh;
    }

    protected override void PerformInternalBehaviour()
    {
        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        _gadget.Resize(_dw, _dh);
    }
}
