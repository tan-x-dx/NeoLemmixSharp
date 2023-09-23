using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillTrackingData
{
    public LemmingSkill Skill { get; }
    public Team Team { get; }
    public int SkillCount { get; private set; }

    public bool IsInfinite => SkillCount == Global.InfiniteSkillCount;

    public SkillTrackingData(LemmingSkill skill, Team team, int skillCount)
    {
        Skill = skill;
        Team = team;
        SetSkillCount(skillCount);
    }

    public void ChangeSkillCount(int delta)
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
        >= Global.InfiniteSkillCount => Skill == ClonerSkill.Instance
            ? 99
            : Global.InfiniteSkillCount,
        < 0 => 0,
        _ => skillCount
    };

    public bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.State.CanHaveSkillsAssigned &&
               Team == lemming.State.TeamAffiliation &&
               Skill.CanAssignToLemming(lemming);
    }
}