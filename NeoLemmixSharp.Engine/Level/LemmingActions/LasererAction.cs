using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class LasererAction : LemmingAction, IDestructionMask
{
    private const int DistanceCap = 112;

    public static readonly LasererAction Instance = new();

    private static ReadOnlySpan<int> RawOffsetChecksRight =>
    [
        1, -1,
        0, -1,
        1, 0,
        -1, -1,
        -1, -2,
        0, -2,
        1, -2,
        2, -1,
        2, 0,
        2, 2,
        1, 1
    ];

    private static ReadOnlySpan<int> RawOffsetChecksLeft =>
    [
        -1, -1,
        0, -1,
        -1, 0,
        1, -1,
        1, -2,
        0, -2,
        -1, -2,
        -2, -1,
        -2, 0,
        -2, 2,
        -1, 1
    ];

    private static ReadOnlySpan<Point> GetOffsetChecks(FacingDirection facingDirection) => MemoryMarshal
        .Cast<int, Point>(facingDirection == FacingDirection.Right
            ? RawOffsetChecksRight
            : RawOffsetChecksLeft);

    private enum LaserHitType
    {
        None,
        Solid,
        Indestructible,
        OutOfBounds
    }

    private LasererAction()
        : base(
            LemmingActionConstants.LasererActionId,
            LemmingActionConstants.LasererActionName,
            LemmingActionConstants.LasererActionSpriteFileName,
            LemmingActionConstants.LasererAnimationFrames,
            LemmingActionConstants.MaxLasererPhysicsFrames,
            LemmingActionConstants.NonPermanentSkillPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Data.Orientation;
        var lemmingPosition = lemming.Data.AnchorPosition;

        if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var facingDirection = lemming.Data.FacingDirection;
        var dx = facingDirection.DeltaX;
        var target = orientation.Move(lemmingPosition, dx * 2, 5);

        var hit = false;
        var hitUseful = false;

        var offsetChecks = GetOffsetChecks(facingDirection);

        var i = DistanceCap;
        while (i > 0)
        {
            switch (CheckForHit(in gadgetsNearLemming, offsetChecks))
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

        lemming.Data.LaserHitLevelPosition = target;

        if (hit)
        {
            lemming.Data.LaserHit = true;
            TerrainMasks.ApplyLasererMask(lemming, target);
        }
        else
        {
            lemming.Data.LaserHit = false;
        }

        if (hitUseful)
        {
            lemming.Data.LaserRemainTime = 10;
        }
        else
        {
            lemming.Data.LaserRemainTime--;
            if (lemming.Data.LaserRemainTime <= 0)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }

        return true;

        LaserHitType CheckForHit(in GadgetEnumerable gadgetsNearLemming1, ReadOnlySpan<Point> offsetChecks)
        {
            if (LevelScreen.TerrainManager.PositionOutOfBounds(target))
                return LaserHitType.OutOfBounds;

            var result = LaserHitType.None;

            foreach (var offset in offsetChecks)
            {
                var checkLevelPosition = orientation.Move(target, offset.X, offset.Y);

                //  gadgetManager.GetAllGadgetsForPosition(scratchSpaceSpan1, checkLevelPosition, out var gadgetSet);

                if (!PositionIsSolidToLemming(in gadgetsNearLemming1, lemming, checkLevelPosition))
                    continue;

                result = PositionIsIndestructibleToLemming(in gadgetsNearLemming1, lemming, this, checkLevelPosition) &&
                         result != LaserHitType.Solid
                    ? LaserHitType.Indestructible
                    : LaserHitType.Solid;
            }

            return result;
        }
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.Data.LaserRemainTime = 10;
    }

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection) => FencerAction.Instance.CanDestroyPixel(pixelType, orientation, facingDirection); // Defer to whatever the fencer does, since the logic is the same!
}
