namespace NeoLemmixSharp.IO.Data.Level;

public sealed class SkillSetData
{
    public required int SkillId { get; init; }
    public required int NumberOfSkills { get; init; }
    public required int TeamId { get; init; }

    internal SkillSetData()
    {
    }
}