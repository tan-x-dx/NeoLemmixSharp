using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.Level.Objectives.Requirements;

public sealed class BasicSkillSetRequirement : ISkillSetRequirement
{
    private readonly SkillSetData[] _skillSetData;

    public ReadOnlySpan<SkillSetData> SkillSetData => new(_skillSetData);

    public bool IsSatisfied { get; private set; }
    public bool IsFailed { get; private set; }

    public BasicSkillSetRequirement(SkillSetData[] skillSetData)
    {
        _skillSetData = skillSetData;
    }

    public void RecheckCriteria()
    {

    }
}