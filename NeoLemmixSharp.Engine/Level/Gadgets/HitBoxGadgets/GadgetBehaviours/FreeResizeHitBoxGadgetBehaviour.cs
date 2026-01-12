using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;

public sealed class FreeResizeHitBoxGadgetBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper _tickCount;
    private readonly GadgetIdentifier _gadgetIdentifier;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private ref int TickCountRef => ref _tickCount.IntValue;

    public FreeResizeHitBoxGadgetBehaviour(
        nint dataHandle,
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        int dw,
        int dh)
        : base(GadgetBehaviourType.GadgetFreeResize)
    {
        _tickCount = new PointerWrapper(dataHandle);
        _tickDelay = tickDelay;
        _gadgetIdentifier = gadgetIdentifier;
        _dw = dw;
        _dh = dh;
    }

    protected override void PerformInternalBehaviour(int _)
    {
        if (TickCountRef < _tickDelay)
        {
            TickCountRef++;
            return;
        }

        TickCountRef = 0;

        var gadget = (HitBoxGadget)LevelScreen.GadgetManager.GetGadget(_gadgetIdentifier.GadgetId);
        gadget.Resize(_dw, _dh);
    }
}
