namespace NeoLemmixSharp.Engine.Level.Objectives.Criteria;

public sealed class TimeRequirement : ObjectiveRequirement
{
    public override bool IsSatisfied()
    {
        return !LevelScreen.LevelTimer.OutOfTime;
    }
}
