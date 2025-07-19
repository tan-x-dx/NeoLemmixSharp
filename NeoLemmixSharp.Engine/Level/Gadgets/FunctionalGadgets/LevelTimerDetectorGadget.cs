using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class LevelTimerDetectorGadget : GadgetBase
{
    public LevelTimerDetectorGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex)
        : base(gadgetName)
    {
    }

    protected override void OnTick()
    {
        throw new NotImplementedException();
    }

    public override GadgetState CurrentState { get; }

    protected override GadgetState GetState(int stateIndex)
    {
        throw new NotImplementedException();
    }

    protected override void OnChangeStates(int currentStateIndex)
    {
        throw new NotImplementedException();
    }
}
