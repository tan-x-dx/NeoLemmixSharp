namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class SuperLemmingModeLevelUpdater : ILevelUpdater
{
    private int _levelUpdateCount;
    private bool _levelUpdateEnabled;

    public bool IsFastForwards => true;
    public void ToggleFastForwards()
    {
        // Do nothing - always fast forwards
    }

    public void UpdateLemming(Lemming lemming)
    {
        if (!lemming.ShouldTick)
            return;

        if (lemming.FastForwardTime > 0 || _levelUpdateEnabled)
        {
            lemming.Tick();
        }
    }

    public void Update()
    {
        _levelUpdateCount++;
        _levelUpdateEnabled = (_levelUpdateCount & 1) == 1;
    }
}