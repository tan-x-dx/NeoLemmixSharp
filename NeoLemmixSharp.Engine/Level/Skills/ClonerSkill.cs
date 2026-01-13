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
            LemmingSkillConstants.ClonerSkillId,
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

        var newLemmingCurrentAction = clonedLemming.CurrentAction;

        // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
        if (newLemmingCurrentAction == LemmingActionConstants.MinerActionId)
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
            (newLemmingCurrentAction == LemmingActionConstants.BuilderActionId ||
             newLemmingCurrentAction == LemmingActionConstants.PlatformerActionId))
        {
            BuilderAction.LayBrick(clonedLemming);
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(JumperAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(LasererAction.Instance);
        result.Add(SwimmerAction.Instance);
        result.Add(GliderAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(AscenderAction.Instance);
        result.Add(FallerAction.Instance);
        result.Add(FloaterAction.Instance);
        result.Add(DisarmerAction.Instance);
        result.Add(ShimmierAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(ReacherAction.Instance);

        return result;
    }
}
