namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public abstract class GadgetBehaviour
{
    private readonly int _maxTriggerCountPerTick;

    private int _currentTickTriggerCount;

    protected GadgetBehaviour(
        int maxTriggerCountPerUpdate)
    {
        _maxTriggerCountPerTick = maxTriggerCountPerUpdate;
    }

    public void Reset()
    {
        _currentTickTriggerCount = 0;
    }

    private bool HasReachedMaxTriggerCount() => _currentTickTriggerCount >= _maxTriggerCountPerTick;

    private void RegisterTrigger() => _currentTickTriggerCount++;

    public void PerformBehaviour()
    {
        if (HasReachedMaxTriggerCount())
            return;

        PerformInternalBehaviour();
        RegisterTrigger();
    }

    protected abstract void PerformInternalBehaviour();
}
