namespace NeoLemmixSharp.Engine.Level.Objectives.Requirements;

public sealed class SaveRequirement : IObjectiveRequirement
{
    private readonly int _saveRequirement;

    public bool IsSatisfied { get; private set; }
    public bool IsFailed { get; private set; }

    public SaveRequirement(int saveRequirement)
    {
        _saveRequirement = saveRequirement;
    }

    public void RecheckCriteria()
    {
        var lemmingManager = LevelScreen.LemmingManager;
        IsSatisfied = lemmingManager.LemmingsSaved >= _saveRequirement;
        IsFailed = lemmingManager.NumberOfItems - lemmingManager.LemmingsRemoved < _saveRequirement - lemmingManager.LemmingsSaved;
    }
}