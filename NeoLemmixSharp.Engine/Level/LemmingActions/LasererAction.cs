using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Runtime.InteropServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class LasererAction
{
    public static IDestructionMask DestructionMask => FencerAction.DestructionMask; // Defer to whatever the fencer does, since the logic is the same!

    private const int DistanceCap = 112;

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

    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.AnchorPosition;

        if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
        {
            FallerAction.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;
        var target = orientation.Move(lemmingPosition, dx * 2, 5);

        var hit = false;
        var hitUseful = false;

        var offsetChecks = GetOffsetChecks(facingDirection);

        var i = DistanceCap;

        do
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
        } while (i > 0);

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
                WalkerAction.TransitionLemmingToAction(lemming, false);
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

                result = PositionIsIndestructibleToLemming(in gadgetsNearLemming1, lemming, DestructionMask, checkLevelPosition) &&
                         result != LaserHitType.Solid
                    ? LaserHitType.Indestructible
                    : LaserHitType.Solid;
            }

            return result;
        }
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LemmingActionType.LasererAction.DoMainTransitionActions(lemming, turnAround);

        lemming.LaserRemainTime = 10;
    }
}
