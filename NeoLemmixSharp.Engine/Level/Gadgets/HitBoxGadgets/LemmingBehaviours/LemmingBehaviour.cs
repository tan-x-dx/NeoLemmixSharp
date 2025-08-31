using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public abstract class LemmingBehaviour : GadgetBehaviour
{
    public LemmingBehaviourType LemmingActionType { get; }

    protected LemmingBehaviour(LemmingBehaviourType lemmingActionType)
        : base(GadgetBehaviourType.LemmingBehaviour)
    {
        LemmingActionType = lemmingActionType;
    }
}
