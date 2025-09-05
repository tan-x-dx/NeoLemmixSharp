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
    public readonly Orientation LemmingOrientation;
    public readonly FacingDirection LemmingFacingDirection;

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
        LemmingPosition = lemming.Data.AnchorPosition;
        LemmingOrientation = lemming.Data.Orientation;
        LemmingFacingDirection = lemming.Data.FacingDirection;
    }
}
