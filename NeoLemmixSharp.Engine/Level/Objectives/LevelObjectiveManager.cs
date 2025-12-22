namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjectiveManager
{
    private readonly LevelObjective[] _levelObjectives;
    public LevelObjective MainLevelObjective { get; }

    public LevelObjectiveManager(LevelObjective[] levelObjectives, int indexOfPrimaryLevelObjective)
    {
        _levelObjectives = levelObjectives;
        MainLevelObjective = levelObjectives[indexOfPrimaryLevelObjective];
    }

    public void RecheckLevelObjective()
    {
        foreach (var levelObjective in _levelObjectives)
        {
            levelObjective.RecheckLevelObjective();
        }
    }
}
