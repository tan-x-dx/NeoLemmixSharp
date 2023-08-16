namespace NeoLemmixSharp.Engine.Engine.Updates;

public sealed class UpdateScheduler
{
    private readonly IFrameUpdater _standardFrameUpdater;
    private readonly IFrameUpdater _fastForwardFrameUpdater;

    public UpdateScheduler(bool superLemmingMode)
    {
        _fastForwardFrameUpdater = new FastForwardsFrameUpdater();
        _standardFrameUpdater = superLemmingMode
            ? _fastForwardFrameUpdater
            : new StandardFrameUpdater();

        CurrentlySelectedFrameUpdater = _standardFrameUpdater;
    }

    public IFrameUpdater CurrentlySelectedFrameUpdater { get; private set; }

    public void Tick()
    {
        CurrentlySelectedFrameUpdater.Update();
    }

    public void SetFastSpeed()
    {
        CurrentlySelectedFrameUpdater = _fastForwardFrameUpdater;
    }

    public void SetNormalSpeed()
    {
        CurrentlySelectedFrameUpdater = _standardFrameUpdater;
    }
}