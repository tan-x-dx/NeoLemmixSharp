namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjective
{
    private readonly IObjectiveRequirement[] _requirements;

    public LevelObjective(IObjectiveRequirement[] requirements)
    {
        _requirements = requirements;
    }

    public bool ObjectiveIsComplete()
    {
        var result = true;

        foreach (var requirement in _requirements)
        {
            result &= requirement.IsSatisfied;
        }

        return result;
    }
}