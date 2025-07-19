namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public abstract class GadgetBehaviour
{
    private readonly int _maxTriggerCountPerTick;

    private int _currentTickTriggerCount;

    protected GadgetBehaviour(int maxTriggerCountPerUpdate)
    {
        _maxTriggerCountPerTick = maxTriggerCountPerUpdate;
    }

    public void Reset()
    {
        _currentTickTriggerCount = 0;
    }

    protected bool HasReachedMaxTriggerCount() => _currentTickTriggerCount >= _maxTriggerCountPerTick;

    protected void RegisterTrigger() => _currentTickTriggerCount++;
}
