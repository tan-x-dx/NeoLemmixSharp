using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Skills;
using NeoLemmixSharp.Engine.Engine.Teams;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class SkillSetManager : IComparer<SkillSetManager.SkillTrackingData>
{
    private readonly SkillTrackingData[] _skillDataList;

    public SkillSetManager(ICollection<SkillSetData> skillSetData)
    {
        _skillDataList = CreateSkillDataList(skillSetData);
    }

    public int TotalNumberOfSkills => _skillDataList.Length;

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

    public bool SkillIsAvailable(int skillDataId)
    {
        if (skillDataId == -1)
            return false;

        return _skillDataList[skillDataId].SkillCount > 0;
    }

    public int NumberOfSkillsAvailable(int skillDataId)
    {
        if (skillDataId == -1)
            return 0;

        return _skillDataList[skillDataId].SkillCount;
    }

    public bool SkillCanBeAssignedToLemming(int skillDataId, Lemming lemming)
    {
        if (skillDataId == -1)
            return false;

        var skillData = _skillDataList[skillDataId];
        return skillData.CanAssignToLemming(lemming);
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

    private sealed class SkillTrackingData
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

        public bool CanAssignToLemming(Lemming lemming)
        {
            return lemming.State.CanHaveSkillsAssigned &&
                   Team == lemming.State.TeamAffiliation &&
                   Skill.CanAssignToLemming(lemming);
        }
    }
}