using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillSetManager : IComparer<SkillTrackingData>
{
    private readonly SkillTrackingData[] _skillTrackingDataList;

    public SkillSetManager(IEnumerable<SkillSetData> skillSetData)
    {
        _skillTrackingDataList = CreateSkillDataList(skillSetData);
    }

    public int TotalNumberOfSkills => _skillTrackingDataList.Length;

    private SkillTrackingData[] CreateSkillDataList(IEnumerable<SkillSetData> skillSetData)
    {
        return skillSetData
            .Select(CreateFromSkillSetData)
            .Order(this)
            .ToArray();
    }

    private static SkillTrackingData CreateFromSkillSetData(SkillSetData skillSetData)
    {
        var lemmingSkill = GetSkillByName(skillSetData.SkillName);
        var team = Team.AllItems[skillSetData.TeamId];

        return new SkillTrackingData(lemmingSkill, team, skillSetData.NumberOfSkills);
    }

    private static LemmingSkill GetSkillByName(string name)
    {
        foreach (var lemmingSkill in LemmingSkill.AllItems)
        {
            if (string.Equals(name, lemmingSkill.LemmingSkillName))
                return lemmingSkill;
        }

        throw new ArgumentException("Unknown skill name", nameof(name));
    }

    public SkillTrackingData? GetSkillTrackingData(int skillDataId)
    {
        if (skillDataId == -1)
            return null;

        return _skillTrackingDataList[skillDataId];
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
}