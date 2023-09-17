using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class SkillSetData
{
    public required LemmingSkill Skill { get; init; }
    public int NumberOfSkills { get; init; }
    public int TeamId { get; init; }
}