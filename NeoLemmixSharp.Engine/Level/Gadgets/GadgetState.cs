using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetState
{
    public required GadgetStateName StateName { get; init; }
    public required GadgetTrigger[] GadgetTriggers { private get; init; }

    public void Tick(GadgetBase parentGadget)
    {
        foreach (var gadgetTrigger in GadgetTriggers)
        {
            gadgetTrigger.DetectTrigger(parentGadget);
        }
    }

    public abstract void OnTransitionFrom();
    public abstract void OnTransitionTo();

    public abstract GadgetRenderer Renderer { get; }

    public sealed override string ToString() => StateName.ToString();
}
