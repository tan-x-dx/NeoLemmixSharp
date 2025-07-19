using NeoLemmixSharp.Engine.Level.Gadgets.Triggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetState
{
    private readonly GadgetTrigger[] _gadgetTriggers;

    protected GadgetState(GadgetTrigger[] gadgetTriggers)
    {
        _gadgetTriggers = gadgetTriggers;
    }

    public void Tick()
    {
        foreach (var gadgetTrigger in _gadgetTriggers)
        {
            gadgetTrigger.OnNewTick();
        }

        OnTick();
    }

    protected abstract void OnTick();

    public abstract void OnTransitionFrom();
    public abstract void OnTransitionTo();

    public abstract GadgetRenderer Renderer { get; }
}
