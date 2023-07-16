using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class WalkerSkill : LemmingSkill
{
    public static WalkerSkill Instance { get; } = new();

    private WalkerSkill()
    {
    }

    public override int Id => 0;
    public override string LemmingSkillName => "walker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;

        var lemmingPosition = lemming.LevelPosition;

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        if (lemming.CurrentAction == BuilderAction.Instance &&
            Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(lemmingPosition, 1), lemming) &&
            !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemmingPosition, dx), lemming))
        {
            lemmingPosition = lemming.Orientation.MoveUp(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;
        }

        // Turn around walking lem, if assigned a walker
        if (lemming.CurrentAction == WalkerAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection);

            // Special treatment if in one-way-field facing the wrong direction
            // see http://www.lemmingsforums.net/index.php?topic=2640.0
            var facingDirectionAsOrientation = lemming.FacingDirection.ConvertToRelativeOrientation(lemming.Orientation);

            if (false/*Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.ForceDirection, lemmingPosition, facingDirectionAsOrientation.GetOpposite())*/)
            {
                // Go one back to cancel the horizontal offset in WalkerAction's update method.
                // unless the Lem will fall down (which is handles already in Transition)
                if (Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
                {
                    lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx);
                    lemming.LevelPosition = lemmingPosition;
                }
            }
        }

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return BlockerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return BuilderAction.Instance;
        yield return PlatformerAction.Instance;
        yield return StackerAction.Instance;
        yield return ShimmierAction.Instance;
        yield return LasererAction.Instance;
        yield return ReacherAction.Instance;
        yield return ShruggerAction.Instance;
    }
}