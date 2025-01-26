using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SkillTrackingData : ISnapshotDataConvertible<SkillSetSnapshotData>
{
    public int SkillTrackingDataId { get; }
    public LemmingSkill Skill { get; }
    public Team Team { get; }
    public int SkillCount { get; private set; }

    public bool IsInfinite => SkillCount == EngineConstants.InfiniteSkillCount;

    public SkillTrackingData(int skillTrackingDataId, LemmingSkill skill, Team team, int skillCount)
    {
        SkillTrackingDataId = skillTrackingDataId;
        Skill = skill;
        Team = team;
        SetSkillCount(skillCount);
    }

    public void ChangeSkillCount(int delta)
    {
        if (IsInfinite)
            return;

        var newSkillCount = SkillCount + delta;
        SkillCount = Math.Clamp(newSkillCount, 0, EngineConstants.InfiniteSkillCount - 1);
    }

    public void SetSkillCount(int skillCount)
    {
        var upperBound = Skill == ClonerSkill.Instance
            ? EngineConstants.InfiniteSkillCount - 1
            : EngineConstants.InfiniteSkillCount;

        SkillCount = Math.Clamp(skillCount, 0, upperBound);
    }

    public bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.State.CanHaveSkillsAssigned &&
               Team == lemming.State.TeamAffiliation &&
               Skill.CanAssignToLemming(lemming);
    }

    public void WriteToSnapshotData(out SkillSetSnapshotData snapshotData)
    {
        snapshotData = new SkillSetSnapshotData(SkillTrackingDataId, SkillCount);
    }

    public void SetFromSnapshotData(in SkillSetSnapshotData snapshotData)
    {
        if (snapshotData.SkillTrackingDataId != SkillTrackingDataId)
            throw new InvalidOperationException("Mismatching IDs!");

        SkillCount = snapshotData.SkillCount;
    }
}