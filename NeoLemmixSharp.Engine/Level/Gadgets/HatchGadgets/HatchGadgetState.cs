using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchGadgetState : GadgetState
{
    protected override void OnTick()
    {
        throw new NotImplementedException();
    }

    public override void OnTransitionFrom()
    {
        throw new NotImplementedException();
    }

    public override void OnTransitionTo()
    {
        throw new NotImplementedException();
    }

    public override GadgetRenderer Renderer { get; }
}
