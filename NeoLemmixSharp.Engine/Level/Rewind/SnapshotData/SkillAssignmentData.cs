using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct SkillAssignmentData : ILevelEventData
{
    public readonly int Tick;
    public readonly int SkillId;
    public readonly int TribeId;

    public readonly int LemmingId;
    public readonly Point LemmingPosition;
    public readonly int LemmingOrientationRotNum;
    public readonly int LemmingFacingDirectionId;

    public int TickNumber => Tick;

    public SkillAssignmentData(
        int tick,
        Lemming lemming,
        LemmingSkill lemmingSkill)
    {
        Tick = tick;
        SkillId = lemmingSkill.Id;
        TribeId = lemming.State.TribeAffiliation.Id;

        LemmingId = lemming.Id;
        LemmingPosition = lemming.AnchorPosition;
        LemmingOrientationRotNum = lemming.Orientation.RotNum;
        LemmingFacingDirectionId = lemming.FacingDirection.Id;
    }
}
