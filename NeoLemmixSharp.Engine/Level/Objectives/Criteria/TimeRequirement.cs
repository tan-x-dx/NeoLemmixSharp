namespace NeoLemmixSharp.Engine.Level.Objectives.Criteria;

public sealed class TimeRequirement : ObjectiveCriterion
{
    public override bool IsSatisfied()
    {
        return !LevelScreen.LevelTimer.OutOfTime;
    }
}
