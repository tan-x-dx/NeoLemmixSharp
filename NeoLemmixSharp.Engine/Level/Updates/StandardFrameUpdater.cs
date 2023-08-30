using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Updates;

public sealed class StandardFrameUpdater : IFrameUpdater
{
    private readonly ItemWrapper<int> _elapsedTicks;

    private int _levelUpdateCountModulo3;
    private bool _doLevelUpdate;

    public StandardFrameUpdater(ItemWrapper<int> elapsedTicks)
    {
        _elapsedTicks = elapsedTicks;
    }

    public UpdateState UpdateState => UpdateState.Normal;

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.ShouldTick && (_doLevelUpdate || lemming.IsFastForward))
        {
            lemming.Tick();
        }
    }

    public bool Tick()
    {
        _elapsedTicks.Item++;
        _levelUpdateCountModulo3 = _elapsedTicks.Item % 3;
        _doLevelUpdate = _levelUpdateCountModulo3 == 0;

        return _doLevelUpdate;
    }
}