using NeoLemmixSharp.Engine.Level.Objectives.Criteria;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjective
{
    private readonly ObjectiveRequirement[] _objectiveCriteria;
    public string Name { get; }

    public bool IsSatisfied { get; private set; }

    public LevelObjective(ObjectiveRequirement[] objectiveCriteria, string name)
    {
        _objectiveCriteria = objectiveCriteria;
        Name = name;
    }

    public void RecheckLevelObjective()
    {
        var allCriteriaSatisfied = true;

        foreach (var objectiveCriteria in _objectiveCriteria)
        {
            allCriteriaSatisfied = objectiveCriteria.IsSatisfied() && allCriteriaSatisfied;
        }

        IsSatisfied = allCriteriaSatisfied;
    }
}
