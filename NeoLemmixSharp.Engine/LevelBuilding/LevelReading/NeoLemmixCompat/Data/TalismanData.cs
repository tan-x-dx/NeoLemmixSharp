using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class TalismanData
{
    public string Title { get; set; }
    public int Id { get; set; }
    public TalismanColor Color { get; set; }

    public int? SaveRequirement { get; set; }
    public int? TimeLimitInSeconds { get; set; }
    public int? AllSkillLimit { get; set; }

    public SimpleDictionary<LemmingSkillHasher, LemmingSkillBitBuffer, LemmingSkill, int> SkillLimits { get; } = LemmingSkillHasher.CreateSimpleDictionary<int>();

    public LevelObjective ToLevelObjective(LevelData levelData)
    {
        var objectiveRequirementsList = new List<IObjectiveRequirement>();

        if (SaveRequirement.HasValue)
        {
            objectiveRequirementsList.Add(new SaveRequirement(SaveRequirement.Value));
        }

        if (TimeLimitInSeconds.HasValue)
        {
            objectiveRequirementsList.Add(new TimeRequirement(TimeLimitInSeconds.Value));
        }

        objectiveRequirementsList.Add(new BasicSkillSetRequirement(GetSkillSetData(levelData)));

        return new LevelObjective(
            Id,
            Title,
            objectiveRequirementsList.ToArray());
    }

    private SkillSetData[] GetSkillSetData(LevelData levelData)
    {
        var originalSkillSetData = levelData.LevelObjectives.Find(lo => lo.LevelObjectiveId == 0)!.SkillSetData;

        var result = new List<SkillSetData>();

        foreach (var originalSkillSetDatum in originalSkillSetData)
        {
            if (SkillLimits.TryGetValue(originalSkillSetDatum.Skill, out var skillLimit))
            {
                if (originalSkillSetDatum.Skill == ClonerSkill.Instance && skillLimit == EngineConstants.InfiniteSkillCount)
                {
                    skillLimit = EngineConstants.InfiniteSkillCount - 1;
                }

                result.Add(new SkillSetData
                {
                    Skill = originalSkillSetDatum.Skill,
                    NumberOfSkills = skillLimit,
                    TeamId = originalSkillSetDatum.TeamId
                });
            }
            else
            {
                result.Add(originalSkillSetDatum);
            }
        }

        return result.ToArray();
    }
}

public enum TalismanColor
{
    Bronze,
    Silver,
    Gold
}