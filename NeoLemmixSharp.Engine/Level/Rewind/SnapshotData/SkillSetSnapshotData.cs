namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct SkillSetSnapshotData
{
    public readonly int SkillTrackingDataId;
    public readonly int SkillCount;

    public SkillSetSnapshotData(int skillTrackingDataId, int skillCount)
    {
        SkillTrackingDataId = skillTrackingDataId;
        SkillCount = skillCount;
    }
}