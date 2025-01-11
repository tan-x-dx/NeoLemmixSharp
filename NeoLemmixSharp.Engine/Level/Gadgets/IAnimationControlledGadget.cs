using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IAnimationControlledGadget
{
    GadgetLayerRenderer Renderer { get; }
    GadgetStateAnimationController AnimationController { get; }
}
