using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Updates;

public sealed class PauseUpdater : IFrameUpdater
{
    private readonly ItemWrapper<int> _elapsedTicks;

    public PauseUpdater(ItemWrapper<int> elapsedTicks)
    {
        _elapsedTicks = elapsedTicks;
    }

    public UpdateState UpdateState => UpdateState.Paused;

    public bool UpdateGadget(GadgetBase gadget) => false;

    public bool UpdateLemming(Lemming lemming) => false;

    public bool Tick() => false;
}