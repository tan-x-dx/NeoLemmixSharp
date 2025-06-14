using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Objectives;

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
        var lemmingSkill = LemmingSkill.AllItems[skillSetData.SkillId];
        var tribe = LevelScreen.TribeManager.AllItems[skillSetData.TribeId];

        return new SkillTrackingData(id, lemmingSkill, tribe, 0);
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
            else if (skillTrackingDatum.SkillCount == 0)
            {
                result += IoConstants.AssumedSkillCountsFromPickups;
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
                      skillTrackingData.Tribe.Id == EngineConstants.ClassicTribeId; // only classic tribe
        }

        return result;
    }

    public SkillTrackingData? GetSkillTrackingData(int skillDataId)
    {
        if ((uint)skillDataId >= (uint)_skillTrackingDataList.Length)
            return null;

        return _skillTrackingDataList[skillDataId];
    }

    public SkillTrackingData? GetSkillTrackingData(int skillId, int tribeId)
    {
        foreach (var skillTrackingData in _skillTrackingDataList)
        {
            if (skillTrackingData.Skill.Id == skillId &&
               skillTrackingData.Tribe.Id == tribeId)
                return skillTrackingData;
        }

        return null;
    }

    public void SetSkillCount(LemmingSkill lemmingSkill, Tribe? tribe, int value, bool isDelta)
    {
        foreach (var skillTrackingData in AllItems)
        {
            if (skillTrackingData.Skill != lemmingSkill ||
                (tribe is not null && skillTrackingData.Tribe != tribe))
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

        var tribeComparison = x.Tribe.Id.CompareTo(y.Tribe.Id);
        if (tribeComparison != 0)
            return tribeComparison;

        var skillComparison = x.Skill.Id.CompareTo(y.Skill.Id);
        return skillComparison;
    }

    public void Dispose()
    {
        Array.Clear(_skillTrackingDataList);
    }
}