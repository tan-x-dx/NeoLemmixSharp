using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.LemmingBehaviours;

public abstract class LemmingBehaviour
{
    private readonly int _maxTriggerCountPerTick;

    private int _currentTickTriggerCount;

    public LemmingBehaviourType LemmingActionType { get; }

    protected LemmingBehaviour(LemmingBehaviourType lemmingActionType)
    {
        LemmingActionType = lemmingActionType;
    }

    public void Reset()
    {
        _currentTickTriggerCount = 0;
    }

    protected bool HasReachedMaxTriggerCount() => _currentTickTriggerCount >= _maxTriggerCountPerTick;

    protected void RegisterTrigger() => _currentTickTriggerCount++;

    public abstract void PerformBehaviour(Lemming lemming);
}
