using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class HatchGadget : GadgetBase, IAnimationControlledGadget
{
    private GadgetLayerRenderer _renderer;

    public HatchSpawnData HatchSpawnData { get; }
    public GadgetStateAnimationController AnimationController { get; }

    public HatchGadget(
        int id,
        Orientation orientation,
        HatchSpawnData hatchSpawnData,
        GadgetStateAnimationController animationController)
        : base(id, orientation)
    {
        HatchSpawnData = hatchSpawnData;
        AnimationController = animationController;
    }

    public override GadgetLayerRenderer Renderer => _renderer;

    public bool CanReleaseLemmings()
    {
        return true;
    }
}
