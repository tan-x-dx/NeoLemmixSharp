namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class StandardLevelUpdater : ILevelUpdater
{
    private int _levelUpdateCount = 0;
    private bool _levelUpdateEnabled = false;

    public bool IsFastForwards { get; private set; }
    public void ToggleFastForwards()
    {
        IsFastForwards = !IsFastForwards;
    }

    public void UpdateLemming(Lemming lemming)
    {
        if (!lemming.ShouldTick)
            return;

        if (IsFastForwards)
        {
            lemming.Tick();
            if (lemming.FastForwardTime > 0)
            {
                lemming.Tick();
            }
        }
        else if (lemming.FastForwardTime > 0 || _levelUpdateEnabled)
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