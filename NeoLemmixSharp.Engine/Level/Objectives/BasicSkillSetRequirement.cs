using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class BasicSkillSetRequirement : ISkillSetRequirement
{
    private readonly SkillSetData[] _skillSetData;

    public ReadOnlySpan<SkillSetData> SkillSetData => new(_skillSetData);

    public bool IsSatisfied { get; }
    public bool IsFailed { get; }
    
    public BasicSkillSetRequirement(SkillSetData[] skillSetData)
    {
        _skillSetData = skillSetData;
    }
}