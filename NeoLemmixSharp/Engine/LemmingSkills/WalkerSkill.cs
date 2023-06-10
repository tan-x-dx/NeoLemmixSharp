using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class WalkerSkill : LemmingSkill
{
    public WalkerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 20;
    public override string LemmingSkillName => "walker";
    public override bool IsPermanentSkill => false;

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

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        if (lemming.CurrentAction == BuilderAction.Instance &&
            Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 1)).IsSolidToLemming(lemming) &&
            !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemming.LevelPosition, dx)).IsSolidToLemming(lemming))
        {
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
        }

        // Turn around walking lem, if assigned a walker
        if (lemming.CurrentAction == WalkerAction.Instance)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;

            // Special treatment if in one-way-field facing the wrong direction
            // see http://www.lemmingsforums.net/index.php?topic=2640.0
            if (false) /*(HasTriggerAt(L.LemX, L.LemY, trForceRight, L) and(L.LemDx = -1) ||
                HasTriggerAt(L.LemX, L.LemY, trForceLeft, L) and(L.LemDx = 1))*/
            {
                // Go one back to cancel the Inc(L.LemX, L.LemDx) in HandleWalking
                // unless the Lem will fall down (which is handles already in Transition)
                if (Terrain.GetPixelData(lemming.LevelPosition).IsSolidToLemming(lemming))
                {
                    lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
                }
            }
        }

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }
}