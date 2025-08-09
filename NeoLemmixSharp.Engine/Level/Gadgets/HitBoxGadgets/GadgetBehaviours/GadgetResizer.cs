using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;

public sealed class GadgetResizer2 : GadgetBehaviour
{
    private readonly HitBoxGadget _gadget;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private int _tickCount;

    public GadgetResizer2(
        HitBoxGadget gadget,
        int tickDelay,
        int dw,
        int dh)
        : base(GadgetBehaviourType.GadgetResizeFree)
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
