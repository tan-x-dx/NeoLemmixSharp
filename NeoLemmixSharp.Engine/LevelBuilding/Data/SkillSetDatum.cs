using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class SkillSetDatum
{
    public required LemmingSkill Skill { get; init; }
    public required int NumberOfSkills { get; init; }
    public required int TeamId { get; init; }
}