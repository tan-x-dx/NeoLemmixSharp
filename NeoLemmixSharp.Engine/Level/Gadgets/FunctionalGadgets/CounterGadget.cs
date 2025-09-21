using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class CounterGadget : GadgetBase
{
    private readonly CounterGadgetState _state;

    public CounterGadget() : base(GadgetType.Counter)
    {
    }

    public override CounterGadgetState CurrentState => _state;

    public override void Tick()
    {
        throw new NotImplementedException();
    }

    public override void SetState(int stateIndex)
    {
        throw new NotImplementedException();
    }
}

public sealed class CounterGadgetState : GadgetState
{
    public override GadgetRenderer Renderer { get; }
}
