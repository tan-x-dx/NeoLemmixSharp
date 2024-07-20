using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public interface IObjectiveRequirement
{
    bool IsSatisfied { get; }
    bool IsFailed { get; }
}

public interface ISkillSetRequirement : IObjectiveRequirement
{
    ReadOnlySpan<SkillSetData> SkillSetData { get; }
}