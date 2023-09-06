using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Updates;

public sealed class FastForwardsFrameUpdater : IFrameUpdater
{
    private readonly ItemWrapper<int> _elapsedTicks;

    public FastForwardsFrameUpdater(ItemWrapper<int> elapsedTicks)
    {
        _elapsedTicks = elapsedTicks;
    }

    public UpdateState UpdateState => UpdateState.FastForward;

    public bool UpdateGadget(GadgetBase gadget) => true;

    public bool UpdateLemming(Lemming lemming)
    {
        if (!lemming.IsFastForward)
            return true;

        lemming.Tick();
        lemming.Tick();

        return true;
    }

    public bool Tick()
    {
        _elapsedTicks.Item++;

        return true;
    }
}