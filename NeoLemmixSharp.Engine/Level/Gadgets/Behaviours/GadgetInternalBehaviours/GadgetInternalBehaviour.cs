namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GadgetInternalBehaviours;

public abstract class GadgetInternalBehaviour : GadgetBehaviour
{
    protected GadgetInternalBehaviour()
        : base(1)
    {
    }

    public abstract void PerformBehaviour();
}
