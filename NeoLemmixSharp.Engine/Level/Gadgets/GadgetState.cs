using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetState
{
    public abstract void Tick();
    public abstract void OnTransitionFrom();
    public abstract void OnTransitionTo();

    public abstract GadgetRenderer Renderer { get; }
}
