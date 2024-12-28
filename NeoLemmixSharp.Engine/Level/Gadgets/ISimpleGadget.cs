using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface ISimpleGadget
{
    SimpleGadgetRenderer Renderer { get; set; }
}
