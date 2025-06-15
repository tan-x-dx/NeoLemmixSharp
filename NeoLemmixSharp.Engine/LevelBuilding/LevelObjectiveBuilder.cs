using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.IO.Data.Level.Objectives;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LevelObjectiveBuilder
{
    public static LevelObjectiveManager BuildLevelObjectiveManager(LevelObjectiveData levelObjective)
    {
        return null;
    }

    public static SkillSetManager BuildSkillSetManager(LevelObjectiveData levelObjective)
    {
        throw new NotImplementedException();
    }

    public static LevelTimer BuildLevelTimer(LevelObjectiveManager levelObjectiveManager)
    {
        /*  var primaryObjective = levelObjectiveManager.PrimaryLevelObjective;
          foreach (var requirement in primaryObjective.Requirements)
          {
              if (requirement is TimeRequirement timeRequirement)
                  return LevelTimer.CreateCountDownTimer(timeRequirement.TimeLimitInSeconds);
          }*/

        return LevelTimer.CreateCountUpTimer();
    }
}
