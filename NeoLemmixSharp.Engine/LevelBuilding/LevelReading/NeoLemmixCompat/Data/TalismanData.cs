using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class TalismanData
{
    public string Title { get; set; }
    public int Id { get; set; }
    public TalismanColor Color { get; set; }

    public int? SaveRequirement { get; set; }
    public int? TimeLimitInSeconds { get; set; }
    public int? AllSkillLimit { get; set; }

    public SimpleDictionary<LemmingSkill, int> SkillLimits { get; } = ExtendedEnumTypeComparer<LemmingSkill>.CreateSimpleDictionary<int>();

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

        return new LevelObjective(
            objectiveRequirementsList.ToArray(),
            GetSkillSetData(levelData));
    }

    private List<SkillSetDatum> GetSkillSetData(LevelData levelData)
    {
        var originalSkillSetData = levelData.PrimaryLevelObjective!.SkillSetData;

        var result = new List<SkillSetDatum>();

        foreach (var originalSkillSetDatum in originalSkillSetData)
        {
            if (SkillLimits.TryGetValue(originalSkillSetDatum.Skill, out var skillLimit))
            {
                result.Add(new SkillSetDatum
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

        return result;
    }
}

public enum TalismanColor
{
    Bronze,
    Silver,
    Gold
}