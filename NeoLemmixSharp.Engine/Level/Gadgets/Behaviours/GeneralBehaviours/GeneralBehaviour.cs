namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

public abstract class GeneralBehaviour : GadgetBehaviour
{
    protected GeneralBehaviour()
        : base(1)
    {
    }

    public abstract void PerformBehaviour();
}
