using NeoLemmixSharp.Engine.Engine.Teams;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class SkillTrackingData
{
    public LemmingSkill Skill { get; }
    public Team Team { get; }
    public int SkillCount { get; set; }

    public SkillTrackingData(LemmingSkill skill, Team team, int skillCount)
    {
        Skill = skill;
        Team = team;
        SkillCount = skillCount;
    }
}