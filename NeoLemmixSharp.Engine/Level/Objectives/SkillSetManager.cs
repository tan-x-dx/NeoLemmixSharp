using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class SkillSetManager : IItemManager<SkillTrackingData>, IComparer<SkillTrackingData>, IDisposable
{
    private readonly SkillTrackingData[] _skillTrackingDataList;

    private int _currentTotalSkillLimit;

    public ReadOnlySpan<SkillTrackingData> AllItems => new(_skillTrackingDataList);

    public SkillSetManager(
        SkillTrackingData[] skillTrackingDataList,
        int totalSkillLimit)
    {
        _skillTrackingDataList = skillTrackingDataList;
        _currentTotalSkillLimit = totalSkillLimit;

        Array.Sort(_skillTrackingDataList, this);
    }

    public void UpdateSkillSetData()
    {
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            skillTrackingData.RecalculateEffectiveQuantity(_currentTotalSkillLimit);
        }
    }

    public void RecordUsageOfSkill()
    {
        _currentTotalSkillLimit--;
    }

    public SkillTrackingData? GetSkillTrackingData(int skillDataId)
    {
        if ((uint)skillDataId >= (uint)_skillTrackingDataList.Length)
            return null;

        return _skillTrackingDataList[skillDataId];
    }

    public SkillTrackingData? GetSkillTrackingData(int skillId, int? tribeId)
    {
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            if (skillTrackingData.Skill.Id == skillId &&
                skillTrackingData.Tribe?.Id == tribeId)
                return skillTrackingData;
        }

        return null;
    }

    public void SetSkillCount(LemmingSkill lemmingSkill, Tribe? tribe, int value)
    {
        foreach (var skillTrackingData in AllItems)
        {
            if (skillTrackingData.Skill != lemmingSkill ||
                skillTrackingData.Tribe != tribe)
                continue;

            skillTrackingData.ChangeSkillCount(value);
        }
    }

    public bool HasClassicSkillsOnly()
    {
        var result = true;
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            result &= skillTrackingData.Skill.IsClassicSkill() && // only classic skills
                      skillTrackingData.Tribe is null; // no tribe specified
        }

        return result;
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
                result += IoConstants.AssumedSkillUsageForInfiniteSkillCounts;
            }
            else
            {
                result += skillTrackingDatum.InitialSkillQuantity;
            }
        }

        return result;
    }

    int IComparer<SkillTrackingData>.Compare(SkillTrackingData? x, SkillTrackingData? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var tribeComparison = CompareTribes(x.Tribe, y.Tribe);
        if (tribeComparison != 0)
            return tribeComparison;

        var gt = x.Skill.Id > y.Skill.Id ? 1 : 0;
        var lt = x.Skill.Id < y.Skill.Id ? 1 : 0;
        return gt - lt;

        static int CompareTribes(Tribe? tribeX, Tribe? tribeY)
        {
            if (ReferenceEquals(tribeX, tribeY)) return 0;
            if (tribeX is null) return -1;
            if (tribeY is null) return 1;

            var gt = tribeX.Id > tribeY.Id ? 1 : 0;
            var lt = tribeX.Id < tribeY.Id ? 1 : 0;
            return gt - lt;
        }
    }

    public void Dispose()
    {
        Array.Clear(_skillTrackingDataList);
    }
}
