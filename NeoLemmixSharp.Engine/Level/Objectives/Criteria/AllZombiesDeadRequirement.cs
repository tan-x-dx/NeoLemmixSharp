using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Objectives.Criteria;

public sealed class AllZombiesDeadRequirement : ObjectiveRequirement
{
    public override bool IsSatisfied()
    {
        var lemmingManager = LevelScreen.LemmingManager;

        var anyZombies = false;

        foreach (var lemming in lemmingManager.AllLemmings)
        {
            if (!lemming.IsZombie)
                continue;

            anyZombies = true;

            if (!lemming.IsActive)
                return false;

            var lemmingRemovalReason = lemming.LemmingRemovalReason;
            if (!lemmingRemovalReason.IsDeath())
                return false;
        }

        return anyZombies;
    }
}
