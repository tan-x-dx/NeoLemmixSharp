using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClonerSkill : LemmingSkill
{
    public static readonly ClonerSkill Instance = new();

    private ClonerSkill()
        : base(
            LevelConstants.ClonerSkillId,
            LevelConstants.ClonerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return LevelScreen.LemmingManager.CanCreateNewLemming() &&
               ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        if (!LevelScreen.LemmingManager.TryCreateNewLemming(
                lemming.Orientation,
                lemming.FacingDirection,
                lemming.CurrentAction,
                lemming.State.TeamAffiliation,
                lemming.LevelPosition,
                out var newLemming))
            return;

        newLemming.SetRawDataFromOther(lemming);
        newLemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());

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

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return BuilderAction.Instance;
        yield return MinerAction.Instance;
        yield return JumperAction.Instance;
        yield return StackerAction.Instance;
        yield return LasererAction.Instance;
        yield return SwimmerAction.Instance;
        yield return GliderAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return DiggerAction.Instance;
        yield return AscenderAction.Instance;
        yield return FallerAction.Instance;
        yield return FloaterAction.Instance;
        yield return DisarmerAction.Instance;
        yield return ShimmierAction.Instance;
        yield return ShruggerAction.Instance;
        yield return ReacherAction.Instance;
    }

}