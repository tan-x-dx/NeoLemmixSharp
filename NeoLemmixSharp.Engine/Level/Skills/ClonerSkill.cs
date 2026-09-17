using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class ClonerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return LevelScreen.LemmingManager.CanCreateNewLemmingClone() &&
               LemmingSkillType.ClonerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        if (!LevelScreen.LemmingManager.TryGetNextClonedLemming(out var clonedLemming))
            return;

        clonedLemming.SetRawDataFromOther(lemming);
        clonedLemming.FacingDirection = lemming.FacingDirection.GetOpposite();
        clonedLemming.Initialise();

        var newLemmingCurrentActionType = clonedLemming.CurrentActionType;

        // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
        if (newLemmingCurrentActionType == LemmingActionType.MinerAction)
        {
            if (clonedLemming.PhysicsFrame == 2)
            {
                TerrainMasks.ApplyMinerMask(clonedLemming, 1, 0, 0);
            }
            else if (clonedLemming.PhysicsFrame is >= 3 and < 15)
            {
                var dx = clonedLemming.FacingDirection.DeltaX;
                TerrainMasks.ApplyMinerMask(clonedLemming, 1, -2 * dx, -1);
            }

            return;
        }

        // Required for turned builders not to walk into air
        // For platformers, see http://www.lemmingsforums.net/index.php?topic=2530.0
        if (clonedLemming.PhysicsFrame >= 9 &&
            (newLemmingCurrentActionType == LemmingActionType.BuilderAction ||
             newLemmingCurrentActionType == LemmingActionType.PlatformerAction))
        {
            BuilderAction.LayBrick(clonedLemming);
        }
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.JumperAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.LasererAction;
        yield return LemmingActionType.SwimmerAction;
        yield return LemmingActionType.GliderAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.AscenderAction;
        yield return LemmingActionType.FallerAction;
        yield return LemmingActionType.FloaterAction;
        yield return LemmingActionType.DisarmerAction;
        yield return LemmingActionType.ShimmierAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.ReacherAction;
    }
}
