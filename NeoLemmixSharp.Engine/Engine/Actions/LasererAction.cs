using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class LasererAction : LemmingAction
{
    private const int DistanceCap = 112;
    public const int NumberOfLasererAnimationFrames = 1;

    public static LasererAction Instance { get; } = new();

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

    public override int Id => 15;
    public override string LemmingActionName => "laserer";
    public override int NumberOfAnimationFrames => NumberOfLasererAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        if (!Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var dx = lemming.FacingDirection.DeltaX;
        var target = orientation.Move(lemmingPosition, dx + dx, 5);

        var hit = false;
        var hitUseful = false;

        var offsetChecks = dx == 1
            ? _offsetChecksRight
            : _offsetChecksLeft;

        var i = DistanceCap;
        while (i > 0)
        {
            switch (CheckForHit(lemming, target, orientation, offsetChecks))
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

    private static LaserHitType CheckForHit(
        Lemming lemming,
        LevelPosition target,
        Orientation orientation,
        LevelPosition[] offsetChecks)
    {
        if (Terrain.PositionOutOfBounds(target))
            return LaserHitType.OutOfBounds;

        var result = LaserHitType.None;

        for (var i = 0; i < offsetChecks.Length; i++)
        {
            var checkLevelPosition = orientation.Move(target, offsetChecks[i]);

            if (Terrain.PixelIsSolidToLemming(checkLevelPosition, lemming))
            {
                if (Terrain.PixelIsIndestructibleToLemming(checkLevelPosition, lemming) && result != LaserHitType.Solid)
                {
                    result = LaserHitType.Indestructible;
                }
                else
                {
                    result = LaserHitType.Solid;
                }
            }
        }

        return result;
    }
}