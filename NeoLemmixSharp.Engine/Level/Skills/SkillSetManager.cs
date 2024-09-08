using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillSetManager : IComparer<SkillTrackingData>, IDisposable
{
    private readonly TickOrderedList<SkillAssignmentData> _skillCountChanges;
    private readonly SkillTrackingData[] _skillTrackingDataList;

    public ReadOnlySpan<SkillTrackingData> AllSkillTrackingData => new(_skillTrackingDataList);

    public SkillSetManager(LevelObjective levelObjective)
    {
        _skillTrackingDataList = CreateSkillDataList(levelObjective.SkillSetData);

        var baseNumberOfSkillAssignments = CalculateBaseNumberOfSkillAssignments();

        _skillCountChanges = new TickOrderedList<SkillAssignmentData>(baseNumberOfSkillAssignments);
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

    private static SkillTrackingData CreateFromSkillSetData(SkillSetData skillSetData, int i)
    {
        var lemmingSkill = skillSetData.Skill;
        var team = Team.AllItems[skillSetData.TeamId];

        return new SkillTrackingData(lemmingSkill, team, i, skillSetData.NumberOfSkills);
    }

    private int CalculateBaseNumberOfSkillAssignments()
    {
        var result = 0;

        foreach (var skillTrackingDatum in _skillTrackingDataList)
        {
            // Add one for each item
            result++;

            if (skillTrackingDatum.IsInfinite)
            {
                result += LevelConstants.AssumedSkillUsageForInfiniteSkillCounts;
            }
            else if (skillTrackingDatum.SkillCount == 0)
            {
                result += LevelConstants.AssumedSkillCountsFromPickups;
            }
            else
            {
                result += skillTrackingDatum.SkillCount;
            }
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

    /*  private void RecordSkillAssignment()
      {
          var previousLatestTickWithUpdate = _skillCountChanges.LatestTickWithData();
          var currentLatestTickWithUpdate = LevelScreen.UpdateScheduler.ElapsedTicks;

          if (previousLatestTickWithUpdate != currentLatestTickWithUpdate)
          {
              _firstIndexOfTickUpdates = _skillCountChanges.Count;
          }

          ref var pixelChangeData = ref _skillCountChanges.GetNewDataRef();

          pixelChangeData = new SkillAssignmentData(currentLatestTickWithUpdate, pixel.X, pixel.Y, fromColor, toColor, fromPixelType, toPixelType);
      }*/

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
            result &= skillTrackingData.Skill.IsClassicSkill() && // only classic skills
                      skillTrackingData.Team.Id == LevelConstants.ClassicTeamId; // only classic team
        }

        return result;
    }

    public void RewindBackTo(int tick)
    {
        var skillCountChanges = _skillCountChanges.RewindBackTo(tick);
        if (skillCountChanges.Length == 0)
            return;

        for (var i = skillCountChanges.Length - 1; i >= 0; i--)
        {
            ref readonly var pixelChangeData = ref skillCountChanges[i];


        }
    }

    public void Dispose()
    {
        Array.Clear(_skillTrackingDataList);
    }
}