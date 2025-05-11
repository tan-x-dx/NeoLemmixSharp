using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.IO.Reading.Levels;

internal static class LevelReadingHelpers
{
    public static int CalculateMaxNumberOfClonedLemmings(
        LevelData levelData)
    {
        var numberOfClonerSkillPickups = 0;

        foreach (var gadgetDatum in levelData.AllGadgetData)
        {
            if (gadgetDatum.TryGetProperty(GadgetProperty.SkillId, out var skillId))
            {
                if (skillId == EngineConstants.ClonerSkillId)
                {
                    numberOfClonerSkillPickups += gadgetDatum.GetProperty(GadgetProperty.Count);
                }
            }
        }

        var maxNumberOfClonerSkills = 0;

        /*     foreach (var levelObjective in levelData.LevelObjectives)
             {
                 var numberOfClonerSkillsPerLevelObjective = 0;

                 foreach (var skillSetDatum in levelObjective.SkillSetData)
                 {
                     if (skillSetDatum.Skill == ClonerSkill.Instance)
                     {
                         numberOfClonerSkillsPerLevelObjective += skillSetDatum.NumberOfSkills;
                     }
                 }

                 maxNumberOfClonerSkills = Math.Max(maxNumberOfClonerSkills, numberOfClonerSkillsPerLevelObjective);
             }*/

        return numberOfClonerSkillPickups + maxNumberOfClonerSkills;
    }
}