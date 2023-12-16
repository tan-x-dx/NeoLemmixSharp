using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ShimmierSkill : LemmingSkill
{
    public static readonly ShimmierSkill Instance = new();

    private ShimmierSkill()
    {
    }

    public override int Id => LevelConstants.ShimmierSkillId;
    public override string LemmingSkillName => "shimmier";
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        var terrainManager = LevelConstants.TerrainManager;

        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            var simulationLemming = LemmingManager.SimulateLemming(lemming, true);

            if (simulationLemming.CurrentAction != SliderAction.Instance &&
                (simulationLemming.CurrentAction != FallerAction.Instance ||
                 simulationLemming.FacingDirection != lemming.FacingDirection.GetOpposite()))
                return false;

            var simulationOrientation = simulationLemming.Orientation;
            var simulationPosition = simulationLemming.LevelPosition;

            return terrainManager.PixelIsSolidToLemming(simulationLemming, simulationOrientation.MoveUp(simulationPosition, 9)) ||
                   terrainManager.PixelIsSolidToLemming(simulationLemming, simulationOrientation.MoveUp(simulationPosition, 8));
        }

        if (lemming.CurrentAction == SliderAction.Instance ||
            lemming.CurrentAction == DehoisterAction.Instance)
        {
            var oldAction = lemming.CurrentAction;

            var simulationLemming = LemmingManager.SimulateLemming(lemming, true);

            return simulationLemming.CurrentAction != oldAction &&
                   simulationLemming.FacingDirection == lemming.FacingDirection &&
                   (oldAction != DehoisterAction.Instance || simulationLemming.CurrentAction != SliderAction.Instance);
        }

        if (lemming.CurrentAction != JumperAction.Instance)
            return ActionIsAssignable(lemming);

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        for (var i = -1; i < 4; i++)
        {
            if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9 + i)) &&
                !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8 + i)))
                return true;
        }

        return ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance ||
            lemming.CurrentAction == JumperAction.Instance ||
            lemming.CurrentAction == DehoisterAction.Instance)
        {
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else
        {
            ReacherAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}