using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public interface IGadgetRenderer : IViewportObjectRenderer
{
    GadgetRenderMode RenderMode { get; }
    void SetGadget(GadgetBase gadget);
}