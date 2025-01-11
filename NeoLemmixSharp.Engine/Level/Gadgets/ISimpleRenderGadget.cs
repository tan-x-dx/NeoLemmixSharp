using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface ISimpleRenderGadget
{
    SimpleGadgetRenderer Renderer { get; set; }
}
