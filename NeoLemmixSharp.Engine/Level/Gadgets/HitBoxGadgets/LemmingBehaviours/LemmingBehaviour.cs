using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public abstract class LemmingBehaviour : GadgetBehaviour
{
    public LemmingBehaviourType LemmingActionType { get; }

    protected LemmingBehaviour(LemmingBehaviourType lemmingActionType)
        : base(lemmingActionType.ToGadgetBehaviourType())
    {
        LemmingActionType = lemmingActionType;
    }

    protected sealed override void PerformInternalBehaviour()
    {
    }

    public abstract void PerformBehaviour(Lemming lemming);
}
