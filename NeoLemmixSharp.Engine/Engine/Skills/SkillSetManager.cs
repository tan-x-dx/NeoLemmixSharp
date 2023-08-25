using NeoLemmixSharp.Engine.Engine.Teams;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class SkillSetManager : IComparer<SkillTrackingData>
{
    private readonly SkillTrackingData[] _skillTrackingDataList;

    public SkillSetManager(ICollection<SkillSetData> skillSetData)
    {
        _skillTrackingDataList = CreateSkillDataList(skillSetData);
    }

    public int TotalNumberOfSkills => _skillTrackingDataList.Length;

    private SkillTrackingData[] CreateSkillDataList(ICollection<SkillSetData> skillSetData)
    {
        var tempList = new List<SkillTrackingData>();

        foreach (var x in skillSetData)
        {
            var lemmingSkill = GetSkillByName(x.SkillName);
            var team = Team.AllItems[x.TeamId];

            var item = new SkillTrackingData(lemmingSkill, team, x.NumberOfSkills);
            tempList.Add(item);
        }

        tempList.Sort(this);

        return tempList.ToArray();
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
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var teamComparison = x.Team.Id.CompareTo(y.Team.Id);
        if (teamComparison != 0)
            return teamComparison;

        var skillComparison = x.Skill.Id.CompareTo(y.Skill.Id);
        return skillComparison;
    }
}