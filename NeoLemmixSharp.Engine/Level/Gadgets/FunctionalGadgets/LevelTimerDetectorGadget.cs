using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Triggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget;

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

public sealed class LevelTimerDetectorGadgetState : GadgetState
{
    public LevelTimerDetectorGadgetState(GadgetTrigger[] gadgetTriggers) : base(gadgetTriggers)
    {
    }

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

public sealed class LevelTimerDetectorGadgetTrigger : GadgetTrigger
{
    public LevelTimerDetectorGadgetTrigger(GadgetTriggerName triggerName, GadgetBehaviour[] gadgetBehaviours)
        : base(triggerName, gadgetBehaviours)
    {
    }

    public override void Tick()
    {
        throw new NotImplementedException();
    }
}
