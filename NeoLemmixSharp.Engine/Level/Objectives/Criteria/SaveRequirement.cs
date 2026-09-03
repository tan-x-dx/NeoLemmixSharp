using NeoLemmixSharp.Common.Enums;

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
        var saveCount = GetTribeSaveCount();

        return saveCount >= _saveRequirement;
    }

    private int GetTribeSaveCount()
    {
        var lemmingManager = LevelScreen.LemmingManager;

        var result = 0;

        foreach (var lemming in lemmingManager.AllLemmings)
        {
            if (lemming.TribeId != _tribeId)
                continue;

            if (lemming.LemmingRemovalReason == LemmingRemovalReason.Exit)
                result++;
        }

        return result;
    }
}
