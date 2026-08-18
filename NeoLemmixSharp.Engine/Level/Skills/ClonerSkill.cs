using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClonerSkill : LemmingSkill
{
    public static readonly ClonerSkill Instance = new();

    private ClonerSkill()
        : base(
            LemmingSkillType.ClonerSkill,
            LemmingSkillConstants.ClonerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return LevelScreen.LemmingManager.CanCreateNewLemmingClone() &&
               SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
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

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.JumperAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.LasererAction);
        result.Add(LemmingActionType.SwimmerAction);
        result.Add(LemmingActionType.GliderAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.AscenderAction);
        result.Add(LemmingActionType.FallerAction);
        result.Add(LemmingActionType.FloaterAction);
        result.Add(LemmingActionType.DisarmerAction);
        result.Add(LemmingActionType.ShimmierAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.ReacherAction);

        return result;
    }
}
