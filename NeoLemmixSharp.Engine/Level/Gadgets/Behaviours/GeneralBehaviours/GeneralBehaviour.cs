namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

public abstract class GeneralBehaviour : GadgetBehaviour
{
    protected GeneralBehaviour()
        : base(1)
    {
    }

    public void PerformBehaviour()
    {
        if (HasReachedMaxTriggerCount())
            return;

        PerformInternalBehaviour();
        RegisterTrigger();
    }

    protected abstract void PerformInternalBehaviour();
}
