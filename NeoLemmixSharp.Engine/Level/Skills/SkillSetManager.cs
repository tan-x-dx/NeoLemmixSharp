using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillSetManager : IComparer<SkillTrackingData>, IDisposable
{
    private readonly SkillTrackingData[] _skillTrackingDataList;

    public ReadOnlySpan<SkillTrackingData> AllSkillTrackingData => new(_skillTrackingDataList);

    public SkillSetManager(IEnumerable<SkillSetData> skillSetData)
    {
        _skillTrackingDataList = CreateSkillDataList(skillSetData);
    }

    private SkillTrackingData[] CreateSkillDataList(IEnumerable<SkillSetData> skillSetData)
    {
        return skillSetData
            .Select(CreateFromSkillSetData)
            .Order(this)
            .ToArray();
    }

    private static SkillTrackingData CreateFromSkillSetData(SkillSetData skillSetData, int i)
    {
        var lemmingSkill = skillSetData.Skill;
        var team = Team.AllItems[skillSetData.TeamId];

        return new SkillTrackingData(lemmingSkill, team, i, skillSetData.NumberOfSkills);
    }

    public SkillTrackingData? GetSkillTrackingData(int skillDataId)
    {
        if (skillDataId == -1)
            return null;

        return _skillTrackingDataList[skillDataId];
    }

    public SkillTrackingData? GetSkillTrackingData(int skillId, int teamId)
    {
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            if (skillTrackingData.Skill.Id == skillId &&
               skillTrackingData.Team.Id == teamId)
                return skillTrackingData;
        }

        return null;
    }

    public void SetSkillCount(LemmingSkill lemmingSkill, Team? team, int value, bool isDelta)
    {
        foreach (var skillTrackingData in AllSkillTrackingData)
        {
            if (skillTrackingData.Skill != lemmingSkill ||
                (team is not null && skillTrackingData.Team != team))
                continue;

            if (isDelta)
            {
                skillTrackingData.ChangeSkillCount(value);
            }
            else
            {
                skillTrackingData.SetSkillCount(value);
            }
        }
    }

    int IComparer<SkillTrackingData>.Compare(SkillTrackingData? x, SkillTrackingData? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var teamComparison = x.Team.Id.CompareTo(y.Team.Id);
        if (teamComparison != 0)
            return teamComparison;

        var skillComparison = x.Skill.Id.CompareTo(y.Skill.Id);
        return skillComparison;
    }

    public bool HasClassicSkillsOnly()
    {
        var result = true;
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            result &= skillTrackingData.Skill.IsClassicSkill && // only classic skills
                      skillTrackingData.Team.Id == LevelConstants.ClassicTeamId; // only classic team
        }

        return result;
    }

    public void Dispose()
    {
        Array.Clear(_skillTrackingDataList);
    }
}