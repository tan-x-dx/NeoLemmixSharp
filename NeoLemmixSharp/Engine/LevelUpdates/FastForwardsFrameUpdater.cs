namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class FastForwardsFrameUpdater : IFrameUpdater
{
    private int _levelUpdateCount;

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.ShouldTick)
        {
            lemming.Tick();

            if (lemming.IsFastForward)
            {
                lemming.Tick();
                lemming.Tick();
            }
        }
    }

    public void Update()
    {
        _levelUpdateCount++;
    }
}