using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

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
        int lemmingSkillId)
    {
        Tick = tick;
        SkillId = lemmingSkillId;
        TeamId = lemming.State.TeamAffiliation.Id;

        LemmingId = lemming.Id;
        LemmingPosition = lemming.LevelPosition;
        LemmingOrientationId = lemming.Orientation.RotNum;
        LemmingFacingDirectionId = lemming.FacingDirection.Id;
    }
}