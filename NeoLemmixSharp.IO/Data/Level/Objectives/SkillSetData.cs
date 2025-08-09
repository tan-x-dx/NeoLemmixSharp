namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public readonly struct SkillSetData(int skillId, int tribeId, int initialQuantity)
{
    public readonly int SkillId = skillId;
    public readonly int TribeId = tribeId;
    public readonly int InitialQuantity = initialQuantity;
}
