namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class FastForwardsFrameUpdater : IFrameUpdater
{
    private int _levelUpdateCount;
    private bool _levelUpdateEnabled;

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.ShouldTick && (lemming.FastForwardTime > 0 || _levelUpdateEnabled))
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