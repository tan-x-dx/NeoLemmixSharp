using NeoLemmixSharp.Engine.Engine.Updates;

namespace NeoLemmixSharp.Engine.Engine.Lemmings;

public sealed class LemmingManager
{
    private readonly Lemming[] _lemmings;
    private readonly LevelCursor _levelCursor;
    private readonly UpdateScheduler _updateScheduler;

    public LemmingManager(Lemming[] lemmings, LevelCursor levelCursor, UpdateScheduler updateScheduler)
    {
        _lemmings = lemmings;
        _levelCursor = levelCursor;
        _updateScheduler = updateScheduler;
    }

    public void CheckLemmingsUnderCursor()
    {
        for (var i = 0; i < _lemmings.Length; i++)
        {
            _levelCursor.CheckLemming(_lemmings[i]);
        }
    }

    public void UpdateLemmings()
    {
        for (var i = 0; i < _lemmings.Length; i++)
        {
          //  _updateScheduler.UpdateLemming(_lemmings[i]);
        }
    }
}