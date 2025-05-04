using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

public sealed class SkillSetData
{
    public required LemmingSkill Skill { get; init; }
    public required int NumberOfSkills { get; init; }
    public required int TeamId { get; init; }
}