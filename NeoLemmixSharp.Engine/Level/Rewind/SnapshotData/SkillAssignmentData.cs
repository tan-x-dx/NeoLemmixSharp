using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct SkillAssignmentData : ITickOrderedData
{
    public readonly int Tick;
    public readonly int SkillId;
    public readonly int TeamId;

    public readonly int LemmingId;
    public readonly LevelPosition LemmingPosition;
    public readonly int LemmingOrientationId;
    public readonly int LemmingFacingDirectionId;

    int ITickOrderedData.TickNumber => Tick;

    public SkillAssignmentData(
        int tick,
        Lemming lemming,
        LemmingSkill lemmingSkill)
    {
        Tick = tick;
        SkillId = lemmingSkill.Id;
        TeamId = lemming.State.TeamAffiliation.Id;

        LemmingId = lemming.Id;
        LemmingPosition = lemming.LevelPosition;
        LemmingOrientationId = lemming.Orientation.RotNum;
        LemmingFacingDirectionId = lemming.FacingDirection.Id;
    }

    public SkillAssignmentData(
        int tick,
        LemmingSkill lemmingSkill,
        Team team)
    {
        Tick = tick;
        SkillId = lemmingSkill.Id;
        TeamId = team.Id;

        LemmingId = -1;
        LemmingPosition = new LevelPosition();
        LemmingOrientationId = 0;
        LemmingFacingDirectionId = 0;
    }
}