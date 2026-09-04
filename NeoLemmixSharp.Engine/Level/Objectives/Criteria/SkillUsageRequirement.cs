using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Objectives.Criteria;

public sealed class SkillUsageRequirement : ObjectiveRequirement
{
    private readonly LemmingSkillType _skillType;
    private readonly ComparisonType _comparisonType;
    private readonly int _usageRequirementValue;

    public SkillUsageRequirement(LemmingSkillType skillType, ComparisonType comparisonType, int usageRequirementValue)
    {
        _skillType = skillType;
        _comparisonType = comparisonType;
        _usageRequirementValue = usageRequirementValue;
    }

    public override bool IsSatisfied()
    {
        var usagesOfSkill = GetUsagesOfSkill();

        return _comparisonType.ComparisonMatches(usagesOfSkill, _usageRequirementValue);
    }

    private int GetUsagesOfSkill()
    {
        var rewindManager = LevelScreen.RewindManager;

        var result = 0;

        foreach (var skillAssignment in rewindManager.SkillAssignmentsSoFar)
        {
            if (skillAssignment.SkillType == _skillType)
                result++;
        }

        return result;
    }
}
