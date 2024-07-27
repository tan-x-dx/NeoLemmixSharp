namespace NeoLemmixSharp.Engine.Level.Objectives.Requirements;

public sealed class SaveRequirement : IObjectiveRequirement
{
    private readonly int _saveRequirement;

    public SaveRequirement(int saveRequirement)
    {
        _saveRequirement = saveRequirement;
    }

    public bool IsSatisfied => LevelScreen.LemmingManager.LemmingsSaved >= _saveRequirement;
    public bool IsFailed { get; }
}