using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

public sealed class TalismanData
{
    public string Title { get; set; }
    public int Id { get; set; }
    public TalismanColor Color { get; set; }

    public int? SaveRequirement { get; set; }
    public int? TimeLimitInSeconds { get; set; }
    public int? AllSkillLimit { get; set; }

  /*  public SkillLimitDictionary SkillLimits { get; } = LemmingSkill.CreateBitArrayDictionary<int>();

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
    }*/
}

public enum TalismanColor
{
    Bronze,
    Silver,
    Gold
}