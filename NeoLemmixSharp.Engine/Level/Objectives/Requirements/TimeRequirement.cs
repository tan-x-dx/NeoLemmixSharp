using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Objectives.Requirements;

public sealed class TimeRequirement : IObjectiveRequirement
{
    public int TimeLimitInSeconds { get; }

    public bool IsSatisfied { get; private set; }
    public bool IsFailed { get; private set; }

    public TimeRequirement(int timeLimitInSeconds)
    {
        TimeLimitInSeconds = timeLimitInSeconds;
    }

    public void OnLemmingRemoved(Lemming lemming, LemmingRemovalReason removalReason)
    {
        throw new NotImplementedException();
    }

    public void RecheckCriteria()
    {
        var levelTimer = LevelScreen.LevelTimer;

        if (levelTimer.TotalElapsedSeconds < TimeLimitInSeconds)
        {
            IsSatisfied = true;
            IsFailed = false;
        }
        else
        {
            IsSatisfied = false;
            IsFailed = true;
        }
    }
}