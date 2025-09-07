using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public interface IGadgetRenderer : IViewportObjectRenderer
{
    GadgetRenderMode RenderMode { get; }
}