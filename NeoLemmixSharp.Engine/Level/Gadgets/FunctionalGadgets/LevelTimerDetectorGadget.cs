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

    public override void Tick()
    {
        throw new NotImplementedException();
    }

    public override GadgetState CurrentState => throw new NotImplementedException();

    public override void SetNextState(int stateIndex)
    {
        throw new NotImplementedException();
    }
}
