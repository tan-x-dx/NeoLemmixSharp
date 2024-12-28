using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public interface IGadgetRenderer : IViewportObjectRenderer
{
    GadgetRenderMode RenderMode { get; }
}

public interface INineSliceGadgetRender : IGadgetRenderer
{
    void SetGadget(HitBoxGadget gadget);
}

public interface IControlledAnimationGadgetRenderer : IGadgetRenderer
{
    void SetGadget(HitBoxGadget gadget);
}