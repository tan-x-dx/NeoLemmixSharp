namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjectiveManager
{
    private readonly LevelObjective[] _talismanObjectives;
    public LevelObjective MainLevelObjective { get; }
    public LevelObjective? SelectedTalismanObjective { get; }

    public LevelObjectiveManager(
        LevelObjective mainLevelObjective,
        LevelObjective[] talismanObjectives,
        LevelObjective? selectedTalismanObjective)
    {
        MainLevelObjective = mainLevelObjective;
        _talismanObjectives = talismanObjectives;
        SelectedTalismanObjective = selectedTalismanObjective;
    }

    public void RecheckLevelObjective()
    {
        MainLevelObjective.RecheckLevelObjective();

        foreach (var levelObjective in _talismanObjectives)
        {
            levelObjective.RecheckLevelObjective();
        }
    }
}
