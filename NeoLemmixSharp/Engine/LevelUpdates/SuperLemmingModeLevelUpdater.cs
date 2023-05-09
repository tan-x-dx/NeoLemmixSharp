namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class SuperLemmingModeLevelUpdater : ILevelUpdater
{
    private int _levelUpdateCount = 0;

    public bool IsFastForwards => true;
    public void ToggleFastForwards()
    {
        // Do nothing - always fast forwards
    }

    public void UpdateLemming(Lemming lemming)
    {
        if (!lemming.ShouldTick)
            return;

        lemming.Tick();
        if (lemming.FastForwardTime > 0)
        {
            lemming.Tick();
        }
    }

    public void Update()
    {
        _levelUpdateCount++;
    }
}