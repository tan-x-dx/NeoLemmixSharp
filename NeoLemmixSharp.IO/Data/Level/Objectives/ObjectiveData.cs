namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public sealed class ObjectiveData
{
    public required string ObjectiveName { get; init; }

    public required SkillSetData[] SkillSetData { get; init; }
    public required ObjectiveCriterion[] ObjectiveCriteria { get; init; }
    public required ObjectiveModifier[] ObjectiveModifiers { get; init; }

    public required TalismanData[] TalismanData { get; init; }
}
