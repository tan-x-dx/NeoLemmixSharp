using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels;

internal static class LevelReadingHelpers
{
    public static int CalculateMaxNumberOfClonedLemmings(
        LevelData levelData)
    {
        var numberOfClonerSkillPickups = 0;

        foreach (var gadgetDatum in levelData.AllGadgetInstanceData)
        {
       /*     if (gadgetDatum.TryGetProperty(GadgetPropertyType.SkillId, out var skillId))
            {
                if (skillId == LemmingSkillConstants.ClonerSkillId)
                {
                    numberOfClonerSkillPickups += gadgetDatum.GetProperty(GadgetPropertyType.Count);
                }
            }*/
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