using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct SkillAssignmentData : IFrameOrderedData
{
    public readonly int LemmingId;
    public readonly LevelPosition LemmingPosition;
    public readonly int LemmingOrientationId;
    public readonly int LemmingFacingDirectionId;
    public readonly int LemmingTeamId;

    public readonly int SkillId;
    public readonly int Frame;

    int IFrameOrderedData.Frame => Frame;

    public SkillAssignmentData(
        Lemming lemming,
        LemmingSkill lemmingSkill,
        int frame)
    {
        LemmingId = lemming.Id;
        LemmingPosition = lemming.LevelPosition;
        LemmingOrientationId = lemming.Orientation.RotNum;
        LemmingFacingDirectionId = lemming.FacingDirection.Id;
        LemmingTeamId = lemming.State.TeamAffiliation.Id;

        SkillId = lemmingSkill.Id;
        Frame = frame;
    }
}