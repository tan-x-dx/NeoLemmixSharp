namespace NeoLemmixSharp.Engine.Level.Objectives.Criteria;

public sealed class TimeRequirement : ObjectiveRequirement
{
    public uint TimeLimitInSeconds { get; }

    public TimeRequirement(uint timeLimitInSeconds)
    {
        TimeLimitInSeconds = timeLimitInSeconds;
    }

    public override bool IsSatisfied()
    {
        return LevelScreen.LevelTimer.EffectiveElapsedSeconds <= TimeLimitInSeconds;
    }
}
