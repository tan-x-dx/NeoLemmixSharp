namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public sealed class TalismanData
{
    public required string TalismanName { get; init; }

    public required ObjectiveCriterion[] OverrideObjectiveCriteria { get; init; }
    public required ObjectiveModifier[] OverrideObjectiveModifiers { get; init; }
}
