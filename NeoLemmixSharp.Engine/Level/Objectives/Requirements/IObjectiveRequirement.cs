using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Objectives.Requirements;

public interface IObjectiveRequirement
{
    bool IsSatisfied { get; }
    bool IsFailed { get; }
}

public interface ISkillSetRequirement : IObjectiveRequirement
{
    ReadOnlySpan<SkillSetData> SkillSetData { get; }
}