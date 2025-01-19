using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;

public interface IAnimationControlledGadget
{
    int Id { get; }
    GadgetLayerRenderer Renderer { get; }
    GadgetStateAnimationController AnimationController { get; }
}
