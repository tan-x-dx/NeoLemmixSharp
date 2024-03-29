﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class LasererAction : LemmingAction, IDestructionMask
{
    private const int DistanceCap = 112;

    public static readonly LasererAction Instance = new();

    private readonly LevelPosition[] _offsetChecksRight =
    {
        new(1, -1),
        new(0, -1),
        new(1, 0),
        new(-1, -1),
        new(-1, -2),
        new(0, -2),
        new(1, -2),
        new(2, -1),
        new(2, 0),
        new(2, 2),
        new(1, 1)
    };

    private readonly LevelPosition[] _offsetChecksLeft =
    {
        new(-1, -1),
        new(0, -1),
        new(-1, 0),
        new(1, -1),
        new(1, -2),
        new(0, -2),
        new(-1, -2),
        new(-2, -1),
        new(-2, 0),
        new(-2, 2),
        new(-1, 1)
    };

    private enum LaserHitType : byte
    {
        None,
        Solid,
        Indestructible,
        OutOfBounds
    }

    private LasererAction()
    {
    }

    public override int Id => LevelConstants.LasererActionId;
    public override string LemmingActionName => "laserer";
    public override int NumberOfAnimationFrames => LevelConstants.LasererAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (!LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;
        var target = orientation.Move(lemmingPosition, dx + dx, 5);

        var hit = false;
        var hitUseful = false;

        var offsetChecks = GetOffsetChecks(facingDirection);

        var i = DistanceCap;
        while (i > 0)
        {
            switch (CheckForHit(lemming, orientation, facingDirection, target, offsetChecks))
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
            // Apply laser mask here
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
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private ReadOnlySpan<LevelPosition> GetOffsetChecks(FacingDirection facingDirection)
    {
        return facingDirection == FacingDirection.RightInstance
            ? new ReadOnlySpan<LevelPosition>(_offsetChecksRight)
            : new ReadOnlySpan<LevelPosition>(_offsetChecksLeft);
    }

    private LaserHitType CheckForHit(
        Lemming lemming,
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition target,
        ReadOnlySpan<LevelPosition> offsetChecks)
    {
        var terrainManager = LevelScreen.TerrainManager;
        if (terrainManager.PositionOutOfBounds(target))
            return LaserHitType.OutOfBounds;

        var result = LaserHitType.None;

        foreach (var position in offsetChecks)
        {
            var checkLevelPosition = orientation.Move(target, position);

            if (!terrainManager.PixelIsSolidToLemming(lemming, checkLevelPosition))
                continue;

            result = result != LaserHitType.Solid &&
                     terrainManager.PixelIsIndestructibleToLemming(lemming, this, checkLevelPosition)
                ? LaserHitType.Indestructible
                : LaserHitType.Solid;
        }

        return result;
    }

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