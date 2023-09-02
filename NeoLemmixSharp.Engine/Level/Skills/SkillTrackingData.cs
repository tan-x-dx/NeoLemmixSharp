using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillTrackingData
{
    public LemmingSkill Skill { get; }
    public Team Team { get; }
    public int SkillCount { get; private set; }

    public bool IsInfinite => SkillCount == GameConstants.InfiniteSkillCount;

    public SkillTrackingData(LemmingSkill skill, Team team, int skillCount)
    {
        Skill = skill;
        Team = team;
        SetSkillCount(skillCount);
    }

    public void DecrementSkillCount()
    {
        if (IsInfinite)
            return;

        if (SkillCount > 0)
        {
            SkillCount--;
        }
    }

    public void SetSkillCount(int skillCount) => SkillCount = skillCount switch
    {
        >= GameConstants.InfiniteSkillCount => Skill == ClonerSkill.Instance
            ? 99
            : GameConstants.InfiniteSkillCount,
        < 0 => 0,
        _ => skillCount
    };
}