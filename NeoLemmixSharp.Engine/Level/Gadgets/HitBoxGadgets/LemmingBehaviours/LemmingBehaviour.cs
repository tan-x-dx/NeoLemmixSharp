using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public abstract class LemmingBehaviour : GadgetBehaviour
{
    public LemmingBehaviourType LemmingActionType { get; }

    protected LemmingBehaviour(LemmingBehaviourType lemmingActionType)
        : base(GadgetBehaviourType.LemmingBehaviour)
    {
        LemmingActionType = lemmingActionType;
    }

    protected sealed override void PerformInternalBehaviour()
    {
        throw new InvalidOperationException("A LemmingBehaviour requires a lemming id!");
    }

    public void PerformBehaviour(int lemmingId)
    {
        if (HasReachedMaxTriggerCount())
            return;

        var lemming = LevelScreen.LemmingManager.GetLemming(lemmingId);
        PerformInternalBehaviour(lemming);
        OnTrigger();
    }

    protected abstract void PerformInternalBehaviour(Lemming lemming);
}
