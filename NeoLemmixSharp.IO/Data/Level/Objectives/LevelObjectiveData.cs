namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public sealed class LevelObjectiveData
{
    public required string ObjectiveName { get; init; }

    public required SkillSetData[] SkillSetData { get; init; }
    public required ObjectiveCriterionData[] ObjectiveCriteria { get; init; }
    public required ObjectiveModifierData[] ObjectiveModifiers { get; init; }

    public required TalismanData[] TalismanData { get; init; }
}
