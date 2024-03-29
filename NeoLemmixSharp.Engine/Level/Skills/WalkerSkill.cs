﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WalkerSkill : LemmingSkill
{
    public static readonly WalkerSkill Instance = new();

    private WalkerSkill()
    {
    }

    public override int Id => LevelConstants.WalkerSkillId;
    public override string LemmingSkillName => "walker";
    public override bool IsClassicSkill => false;

    public override void AssignToLemming(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        var testUp = orientation.MoveUp(lemmingPosition, 1);

        if (lemming.CurrentAction == BuilderAction.Instance &&
            terrainManager.PixelIsSolidToLemming(lemming, testUp) &&
            !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, dx)))
        {
            lemmingPosition = testUp;

            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return;
        }

        if (lemming.CurrentAction != WalkerAction.Instance)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return;
        }

        // Turn around walking lem, if assigned a walker
        lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());

        // Special treatment if in one-way-field facing the wrong direction
        // see http://www.lemmingsforums.net/index.php?topic=2640.0
        var facingDirectionAsOrientation = lemming.FacingDirection.ConvertToRelativeOrientation(orientation);

        if (false /*Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.ForceDirection, lemmingPosition, facingDirectionAsOrientation.GetOpposite())*/
           )
        {
            // Go one back to cancel the horizontal offset in WalkerAction's update method.
            // unless the Lem will fall down (which is handles already in Transition)
            if (terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
            {
                lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            }
        }

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
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