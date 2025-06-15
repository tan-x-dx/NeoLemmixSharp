using NeoLemmixSharp.Engine.Level.Objectives.Criteria;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjectiveManager
{
    private readonly ObjectiveCriterion[] _objectiveCriteria;

    public LevelObjectiveManager(ObjectiveCriterion[] objectiveCriteria)
    {
        _objectiveCriteria = objectiveCriteria;
    }

    public void RecheckLevelObjective()
    {
        var allCriteriaSatisfied = true;

        foreach (var objectiveCriteria in _objectiveCriteria)
        {
            allCriteriaSatisfied = objectiveCriteria.IsSatisfied() && allCriteriaSatisfied;
        }
    }
}
