namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class StandardFrameUpdater : IFrameUpdater
{
    private int _levelUpdateCount;
    private bool _levelUpdateEnabled;

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.ShouldTick && (_levelUpdateEnabled || (lemming.FastForwardTime > 0 && (_levelUpdateCount & 1) == 1)))
        {
            lemming.Tick();
        }
    }

    public void Update()
    {
        _levelUpdateCount++;
        _levelUpdateEnabled = (_levelUpdateCount & 3) == 3;
    }
}