using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.LemmingBehaviours;

public abstract class LemmingBehaviour : GadgetBehaviour
{
    public LemmingBehaviourType LemmingActionType { get; }

    protected LemmingBehaviour(LemmingBehaviourType lemmingActionType)
        : base(1)
    {
        LemmingActionType = lemmingActionType;
    }

    public abstract void PerformBehaviour(Lemming lemming);
}
