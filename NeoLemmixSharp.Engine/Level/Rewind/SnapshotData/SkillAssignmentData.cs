using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct SkillAssignmentData : ITickOrderedData
{
    public readonly int LemmingId;
    public readonly LevelPosition LemmingPosition;
    public readonly int LemmingOrientationId;
    public readonly int LemmingFacingDirectionId;
    public readonly int LemmingTeamId;

    public readonly int SkillId;
    public readonly int Tick;

    int ITickOrderedData.TickNumber => Tick;

    public SkillAssignmentData(
        Lemming lemming,
        LemmingSkill lemmingSkill,
        int tick)
    {
        LemmingId = lemming.Id;
        LemmingPosition = lemming.LevelPosition;
        LemmingOrientationId = lemming.Orientation.RotNum;
        LemmingFacingDirectionId = lemming.FacingDirection.Id;
        LemmingTeamId = lemming.State.TeamAffiliation.Id;

        SkillId = lemmingSkill.Id;
        Tick = tick;
    }
}