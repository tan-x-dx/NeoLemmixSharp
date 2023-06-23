using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.Engine.LevelGadgets;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class WalkerSkill : LemmingSkill
{
    public WalkerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 20;
    public override string LemmingSkillName => "walker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == BlockerAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == ShimmierAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance ||
               lemming.CurrentAction == ReacherAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance;
    }

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
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;

            // Special treatment if in one-way-field facing the wrong direction
            // see http://www.lemmingsforums.net/index.php?topic=2640.0
            var facingDirectionAsOrientation = lemming.FacingDirection.ConvertToRelativeOrientation(lemming.Orientation);

            if (Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.ForceDirection, lemmingPosition, facingDirectionAsOrientation.GetOpposite()))
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
}