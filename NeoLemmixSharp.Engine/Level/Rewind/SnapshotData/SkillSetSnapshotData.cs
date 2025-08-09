namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct SkillSetSnapshotData
{
    public readonly int SkillTrackingDataId;
    public readonly int AdditionalQuantity;
    public readonly int AmountUsed;
    public readonly int CurrentSkillLimit;

    public SkillSetSnapshotData(int skillTrackingDataId, int additionalQuantity, int amountUsed, int currentSkillLimit)
    {
        SkillTrackingDataId = skillTrackingDataId;
        AdditionalQuantity = additionalQuantity;
        AmountUsed = amountUsed;
        CurrentSkillLimit = currentSkillLimit;
    }
}
