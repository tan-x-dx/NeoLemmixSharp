using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class LasererAction : LemmingAction
{
    private const int DistanceCap = 112;
    public const int NumberOfLasererAnimationFrames = 12;

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

    private enum LaserHitType
    {
        None,
        Solid,
        Indestructible,
        OutOfBounds
    }

    private LasererAction()
    {
    }

    public override int ActionId => 18;
    public override string LemmingActionName => "laserer";
    public override int NumberOfAnimationFrames => NumberOfLasererAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var lemmingPosition = lemming.LevelPosition;
        if (!Terrain.GetPixelData(lemmingPosition).IsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        var dx = lemming.FacingDirection.DeltaX;
        var target = lemming.Orientation.Move(lemmingPosition, dx + dx, 5);

        var hit = false;
        var hitUseful = false;

        for (var i = 0; i < DistanceCap; i++)
        {
            switch (CheckForHit(target, lemming.Orientation, dx))
            {
                case LaserHitType.None:
                    target = lemming.Orientation.Move(target, dx, 1);
                    break;
                case LaserHitType.Solid:
                    hit = true;
                    hitUseful = true;
                    break;
                case LaserHitType.Indestructible:
                    hit = true;
                    break;
                case LaserHitType.OutOfBounds:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        lemming.LaserHitLevelPosition = target;

        if (hit)
        {
            lemming.LaserHit = true;
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

    private LaserHitType CheckForHit(
        in LevelPosition target,
        Orientation orientation,
        int dx)
    {
        if (Terrain.PositionOutOfBounds(target))
            return LaserHitType.OutOfBounds;

        var result = LaserHitType.None;

        var offsetChecks = dx == 1
            ? _offsetChecksRight
            : _offsetChecksLeft;

        for (var i = 0; i < offsetChecks.Length; i++)
        {
            var checkLevelPosition = orientation.Move(target, offsetChecks[i]);

            var pixel = Terrain.GetPixelData(checkLevelPosition);

            if (pixel.IsSolid)
            {
                if (pixel.IsSteel && result != LaserHitType.Solid)
                {
                    result = LaserHitType.None;
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