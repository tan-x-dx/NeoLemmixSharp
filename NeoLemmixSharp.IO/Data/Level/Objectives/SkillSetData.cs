namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public readonly struct SkillSetData(int skillId, int initialQuantity, int tribeId)
{
    public readonly int SkillId = skillId;
    public readonly int InitialQuantity = initialQuantity;
    public readonly int TribeId = tribeId;
}
