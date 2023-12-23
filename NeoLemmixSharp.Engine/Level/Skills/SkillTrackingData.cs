using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillTrackingData
{
	public LemmingSkill Skill { get; }
	public Team Team { get; }
	public int SkillTrackingDataId { get; }
	public int SkillCount { get; private set; }

	public bool IsInfinite => SkillCount == LevelConstants.InfiniteSkillCount;

	public SkillTrackingData(LemmingSkill skill, Team team, int skillTrackingDataId, int skillCount)
	{
		Skill = skill;
		Team = team;
		SkillTrackingDataId = skillTrackingDataId;
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
		>= LevelConstants.InfiniteSkillCount => Skill == ClonerSkill.Instance
			? LevelConstants.InfiniteSkillCount - 1
			: LevelConstants.InfiniteSkillCount,
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