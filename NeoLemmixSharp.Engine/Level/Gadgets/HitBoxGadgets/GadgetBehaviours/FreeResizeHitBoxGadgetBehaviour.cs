using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;

public sealed class FreeResizeHitBoxGadgetBehaviour : GadgetBehaviour
{
    private readonly GadgetIdentifier _gadgetIdentifier;

    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private int _tickCount;

    public FreeResizeHitBoxGadgetBehaviour(
        GadgetIdentifier gadgetIdentifier,
        int tickDelay,
        int dw,
        int dh)
        : base(GadgetBehaviourType.GadgetFreeResize)
    {
        _tickDelay = tickDelay;
        _gadgetIdentifier = gadgetIdentifier;
        _dw = dw;
        _dh = dh;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        var gadget = (HitBoxGadget)LevelScreen.GadgetManager.AllItems[_gadgetIdentifier.GadgetId];
        gadget.Resize(_dw, _dh);
    }
}
