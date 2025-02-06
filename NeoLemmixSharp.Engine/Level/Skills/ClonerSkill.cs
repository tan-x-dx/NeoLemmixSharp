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
            EngineConstants.ClonerSkillId,
            EngineConstants.ClonerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return LevelScreen.LemmingManager.CanCreateNewLemmingClone() &&
               ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        if (!LevelScreen.LemmingManager.TryGetNextClonedLemming(out var newLemming))
            return;

        newLemming.SetRawDataFromOther(lemming);
        newLemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
        newLemming.Initialise();

        var newLemmingCurrentAction = newLemming.CurrentAction;

        // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
        if (newLemmingCurrentAction == MinerAction.Instance)
        {
            if (newLemming.PhysicsFrame == 2)
            {
                TerrainMasks.ApplyMinerMask(newLemming, 1, 0, 0);
            }
            else if (newLemming.PhysicsFrame is >= 3 and < 15)
            {
                var dx = newLemming.FacingDirection.DeltaX;
                TerrainMasks.ApplyMinerMask(newLemming, 1, -2 * dx, -1);
            }

            return;
        }

        // Required for turned builders not to walk into air
        // For platformers, see http://www.lemmingsforums.net/index.php?topic=2530.0
        if (newLemming.PhysicsFrame >= 9 &&
            (newLemmingCurrentAction == BuilderAction.Instance ||
             newLemmingCurrentAction == PlatformerAction.Instance))
        {
            BuilderAction.LayBrick(newLemming);
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