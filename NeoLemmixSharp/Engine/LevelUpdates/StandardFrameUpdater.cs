namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class StandardFrameUpdater : IFrameUpdater
{
    private int _levelUpdateCount;
    private int _levelUpdateCountModulo3;
    private bool _levelUpdateEnabled;

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.ShouldTick && (_levelUpdateEnabled || lemming.IsFastForward))
        {
            lemming.Tick();
        }
    }

    public void Update()
    {
        _levelUpdateCount++;
        _levelUpdateCountModulo3 = _levelUpdateCount % 3;
        _levelUpdateEnabled = _levelUpdateCountModulo3 == 0;
    }
}