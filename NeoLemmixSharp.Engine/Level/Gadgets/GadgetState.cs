using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetState
{
    public required GadgetStateName StateName { get; init; }
    private readonly GadgetTrigger[] _gadgetTriggers;

    protected GadgetState(GadgetTrigger[] gadgetTriggers)
    {
        _gadgetTriggers = gadgetTriggers;
    }

    public void Tick()
    {
        foreach (var gadgetTrigger in _gadgetTriggers)
        {
            gadgetTrigger.Tick();
        }

        OnTick();
    }

    protected abstract void OnTick();

    public abstract void OnTransitionFrom();
    public abstract void OnTransitionTo();

    public abstract GadgetRenderer Renderer { get; }

    public sealed override string ToString() => StateName.ToString();
}
