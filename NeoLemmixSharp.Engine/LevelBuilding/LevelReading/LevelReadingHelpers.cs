using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelReadingHelpers
{
    public static int CalculateMaxNumberOfClonedLemmings(
        LevelData levelData)
    {
        var numberOfClonerSkillPickups = 0;

        foreach (var gadgetDatum in levelData.AllGadgetData)
        {
            if (gadgetDatum.TryGetProperty(GadgetProperty.SkillId, out var skillId))
            {
                if (skillId == LevelConstants.ClonerSkillId)
                {
                    numberOfClonerSkillPickups += gadgetDatum.GetProperty(GadgetProperty.Count);
                }
            }
        }

        var maxNumberOfClonerSkills = 0;

        foreach (var levelObjective in levelData.LevelObjectives)
        {
            foreach (var skillSetDatum in levelObjective.SkillSetData)
            {
                if (skillSetDatum.Skill == ClonerSkill.Instance)
                {
                    maxNumberOfClonerSkills = Math.Max(maxNumberOfClonerSkills, skillSetDatum.NumberOfSkills);
                }
            }
        }

        return numberOfClonerSkillPickups + maxNumberOfClonerSkills;
    }
}