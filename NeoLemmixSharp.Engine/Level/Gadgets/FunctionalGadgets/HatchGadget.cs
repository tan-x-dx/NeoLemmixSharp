using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class HatchGadget : GadgetBase
{
    private GadgetLayerRenderer _renderer;

    public HatchSpawnData HatchSpawnData { get; }

    public HatchGadget(
        int id,
        HatchSpawnData hatchSpawnData) : base(id)
    {
        HatchSpawnData = hatchSpawnData;
    }

    public override GadgetLayerRenderer Renderer => _renderer;
}
