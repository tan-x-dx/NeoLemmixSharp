using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class LasererAction : LemmingAction, IDestructionMask
{
    private const int DistanceCap = 112;

    public static readonly LasererAction Instance = new();

    private static readonly LevelPosition[] OffsetChecksRight =
    [
        new LevelPosition(1, -1),
        new LevelPosition(0, -1),
        new LevelPosition(1, 0),
        new LevelPosition(-1, -1),
        new LevelPosition(-1, -2),
        new LevelPosition(0, -2),
        new LevelPosition(1, -2),
        new LevelPosition(2, -1),
        new LevelPosition(2, 0),
        new LevelPosition(2, 2),
        new LevelPosition(1, 1)
    ];

    private static readonly LevelPosition[] OffsetChecksLeft =
    [
        new LevelPosition(-1, -1),
        new LevelPosition(0, -1),
        new LevelPosition(-1, 0),
        new LevelPosition(1, -1),
        new LevelPosition(1, -2),
        new LevelPosition(0, -2),
        new LevelPosition(-1, -2),
        new LevelPosition(-2, -1),
        new LevelPosition(-2, 0),
        new LevelPosition(-2, 2),
        new LevelPosition(-1, 1)
    ];

    private enum LaserHitType
    {
        None,
        Solid,
        Indestructible,
        OutOfBounds
    }

    private LasererAction()
        : base(
            LevelConstants.LasererActionId,
            LevelConstants.LasererActionName,
            LevelConstants.LasererActionSpriteFileName,
            LevelConstants.LasererAnimationFrames,
            LevelConstants.MaxLasererPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllGadgetsForPosition(lemmingPosition);

        if (!PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;
        var target = orientation.Move(lemmingPosition, dx * 2, 5);

        var hit = false;
        var hitUseful = false;

        var offsetChecks = facingDirection == FacingDirection.RightInstance
            ? new ReadOnlySpan<LevelPosition>(OffsetChecksRight)
            : new ReadOnlySpan<LevelPosition>(OffsetChecksLeft);

        var i = DistanceCap;
        while (i > 0)
        {
            switch (CheckForHit(offsetChecks))
            {
                case LaserHitType.None:
                    target = orientation.Move(target, dx, 1);
                    break;
                case LaserHitType.Solid:
                    hit = true;
                    hitUseful = true;
                    goto HitTestConclusive;
                case LaserHitType.Indestructible:
                    hit = true;
                    goto HitTestConclusive;
                case LaserHitType.OutOfBounds:
                    goto HitTestConclusive;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            --i;
        }

        HitTestConclusive:

        lemming.LaserHitLevelPosition = target;

        if (hit)
        {
            lemming.LaserHit = true;
            TerrainMasks.ApplyLasererMask(lemming, target);
        }
        else
        {
            lemming.LaserHit = false;
        }

        if (hitUseful)
        {
            lemming.LaserRemainTime = 10;
        }
        else
        {
            lemming.LaserRemainTime--;
            if (lemming.LaserRemainTime <= 0)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }

        return true;

        LaserHitType CheckForHit(
            ReadOnlySpan<LevelPosition> offsetChecks)
        {
            var terrainManager = LevelScreen.TerrainManager;
            if (terrainManager.PositionOutOfBounds(target))
                return LaserHitType.OutOfBounds;

            var result = LaserHitType.None;

            foreach (var offset in offsetChecks)
            {
                var checkLevelPosition = orientation.Move(target, offset);

                var gadgetSet = LevelScreen.GadgetManager.GetAllGadgetsForPosition(checkLevelPosition);

                if (!PositionIsSolidToLemming(in gadgetSet, lemming, checkLevelPosition))
                    continue;

                result = PositionIsIndestructibleToLemming(in gadgetSet, lemming, this, checkLevelPosition) &&
                         result != LaserHitType.Solid
                    ? LaserHitType.Indestructible
                    : LaserHitType.Solid;
            }

            return result;
        }
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.LaserRemainTime = 10;
    }

    [Pure]
    public bool CanDestroyPixel(
        PixelType pixelType,
        Orientation orientation,
        FacingDirection facingDirection) => FencerAction.Instance.CanDestroyPixel(pixelType, orientation, facingDirection); // Defer to whatever the fencer does, since the logic is the same!
}