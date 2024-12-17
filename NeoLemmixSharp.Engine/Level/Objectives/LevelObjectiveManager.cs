namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjectiveManager
{
    private readonly LevelObjective[] _levelObjectives;

    public LevelObjective PrimaryLevelObjective { get; }

    public LevelObjectiveManager(List<LevelObjective> levelObjectives, int primaryLevelObjectiveId)
    {
        _levelObjectives = levelObjectives.ToArray();

        PrimaryLevelObjective = levelObjectives.Find(lo => lo.LevelObjectiveId == primaryLevelObjectiveId)!;
    }

    public void RecheckCriteria()
    {
        for (var i = 0; i < _levelObjectives.Length; i++)
        {
            var levelObjective = _levelObjectives[i];
            levelObjective.RecheckCriteria();
        }
    }
}
