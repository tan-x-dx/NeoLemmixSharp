namespace NeoLemmixSharp.Engine.LevelUpdates;

public sealed class StandardLevelUpdater : ILevelUpdater
{
    private int _levelUpdateCount;
    private int _levelUpdateMod4;
    private bool _levelUpdateEnabled;
    private int _bitMask = 3;

    public bool IsFastForwards { get; private set; }
    public void ToggleFastForwards()
    {
        IsFastForwards = !IsFastForwards;
        _bitMask = IsFastForwards ? 1 : 3;
        _levelUpdateMod4 = _levelUpdateCount & 3;
        _levelUpdateEnabled = (_levelUpdateMod4 & _bitMask) == _bitMask;
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
        else if (_levelUpdateEnabled || (lemming.FastForwardTime > 0 && (_levelUpdateMod4 & 1) == 1))
        {
            lemming.Tick();
        }
    }

    public void Update()
    {
        _levelUpdateCount++;
        _levelUpdateMod4 = _levelUpdateCount & 3;
        _levelUpdateEnabled = (_levelUpdateMod4 & _bitMask) == _bitMask;
    }
}