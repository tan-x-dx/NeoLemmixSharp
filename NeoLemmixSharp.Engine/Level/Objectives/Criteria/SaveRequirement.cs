namespace NeoLemmixSharp.Engine.Level.Objectives.Criteria;

public sealed class SaveRequirement : ObjectiveRequirement
{
    private readonly int _saveRequirement;
    private readonly int _tribeId;

    public SaveRequirement(int saveRequirement, int tribeId)
    {
        _saveRequirement = saveRequirement;
        _tribeId = tribeId;
    }

    public override bool IsSatisfied()
    {
        var lemmingManager = LevelScreen.LemmingManager;

        return false;
    }
}
