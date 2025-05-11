using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillSetManager : IItemManager<SkillTrackingData>, IComparer<SkillTrackingData>, IDisposable
{
    private readonly SkillTrackingData[] _skillTrackingDataList;

    int IItemManager<SkillTrackingData>.NumberOfItems => _skillTrackingDataList.Length;
    public ReadOnlySpan<SkillTrackingData> AllItems => new(_skillTrackingDataList);

    public SkillSetManager(LevelObjective levelObjective)
    {
        _skillTrackingDataList = CreateSkillDataList(levelObjective.SkillSetData);
    }

    private SkillTrackingData[] CreateSkillDataList(ReadOnlySpan<SkillSetData> skillSetDataSpan)
    {
        var result = new SkillTrackingData[skillSetDataSpan.Length];

        for (var index = 0; index < skillSetDataSpan.Length; index++)
        {
            var skillSetDatum = skillSetDataSpan[index];
            result[index] = CreateFromSkillSetData(skillSetDatum, index);
        }

        Array.Sort(result, this);

        return result;
    }

    private static SkillTrackingData CreateFromSkillSetData(SkillSetData skillSetData, int id)
    {
        var lemmingSkill = skillSetData.Skill;
        var team = LevelScreen.TeamManager.AllItems[skillSetData.TeamId];

        return new SkillTrackingData(id, lemmingSkill, team, skillSetData.NumberOfSkills);
    }

    /// <summary>
    /// <para>
    /// Get a best-guess count for the possible number of skill assignments.
    /// </para>
    /// 
    /// <para>
    /// If it turns out a level has more skill assignments than the initial
    /// guess, this will be fine - the buffer will simply resize as necessary.
    /// This method is an attempt to minimize the number of reallocations of
    /// the skill assign buffer during gameplay.
    /// </para>
    ///
    /// <para>
    /// In the case of levels with finite skills and no pickups, then the number
    /// returned will be the maximum possible amount of skill assignments. This
    /// is a hard upper limit that will be a perfect buffer size for the level.
    /// </para>
    ///
    /// <para>
    /// In other cases, this method will attempt to overestimate the total number
    /// of skill assignments. 
    /// </para>
    /// </summary>
    /// <returns></returns>
    public int CalculateBaseNumberOfSkillAssignments()
    {
        var result = 0;

        foreach (var skillTrackingDatum in _skillTrackingDataList)
        {
            if (skillTrackingDatum.IsInfinite)
            {
                result += EngineConstants.AssumedSkillUsageForInfiniteSkillCounts;
            }
            else if (skillTrackingDatum.SkillCount == 0)
            {
                result += EngineConstants.AssumedSkillCountsFromPickups;
            }
            else
            {
                result += skillTrackingDatum.SkillCount;
            }
        }

        return result;
    }

    public bool HasClassicSkillsOnly()
    {
        var result = true;
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            result &= skillTrackingData.Skill.IsClassicSkill() && // only classic skills
                      skillTrackingData.Team.Id == EngineConstants.ClassicTeamId; // only classic team
        }

        return result;
    }

    public SkillTrackingData? GetSkillTrackingData(int skillDataId)
    {
        if ((uint)skillDataId >= (uint)_skillTrackingDataList.Length)
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
        foreach (var skillTrackingData in AllItems)
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

    public void Dispose()
    {
        Array.Clear(_skillTrackingDataList);
    }
}