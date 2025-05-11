using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public interface IGadgetRenderer : IViewportObjectRenderer
{
    GadgetRenderMode RenderMode { get; }
}