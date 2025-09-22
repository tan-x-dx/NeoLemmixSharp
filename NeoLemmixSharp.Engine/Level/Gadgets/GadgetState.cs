using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetState
{
    public required GadgetStateName StateName { get; init; }
    public required GadgetTrigger[] GadgetTriggers { private get; init; }

    protected GadgetBase ParentGadget = null!;

    public void SetParentGadget(GadgetBase gadget)
    {
        Debug.Assert(ParentGadget is null);

        ParentGadget = gadget;
        foreach (GadgetTrigger trigger in GadgetTriggers)
        {
            trigger.SetParentData(gadget, this);
        }

        OnSetParentGadget();
    }

    protected virtual void OnSetParentGadget() { }

    public void Tick()
    {
        foreach (var gadgetTrigger in GadgetTriggers)
        {
            gadgetTrigger.DetectTrigger();
        }
    }

    public abstract GadgetRenderer Renderer { get; }

    public sealed override string ToString() => StateName.ToString();
}
