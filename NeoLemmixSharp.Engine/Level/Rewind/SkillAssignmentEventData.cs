using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public readonly struct SkillAssignmentEventData : ILevelEventData
{
    public readonly int Tick;
    public readonly int SkillId;
    public readonly int TribeId;

    public readonly int LemmingId;
    public readonly Point LemmingPosition;
    public readonly Orientation LemmingOrientation;
    public readonly FacingDirection LemmingFacingDirection;

    public int TickNumber => Tick;

    public SkillAssignmentEventData(
        int tick,
        Lemming lemming,
        int lemmingSkillId)
    {
        Tick = tick;
        SkillId = lemmingSkillId;
        TribeId = lemming.State.TribeId;

        LemmingId = lemming.Id;
        LemmingPosition = lemming.AnchorPosition;
        LemmingOrientation = lemming.Orientation;
        LemmingFacingDirection = lemming.FacingDirection;
    }
}
