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

    public void UpdateGadget(GadgetBase gadget)
    {
        gadget.Tick();
    }

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.ShouldTick)
        {
            lemming.Tick();

            if (lemming.IsFastForward)
            {
                lemming.Tick();
                lemming.Tick();
            }
        }
    }

    public bool Tick()
    {
        _elapsedTicks.Item++;

        return true;
    }
}