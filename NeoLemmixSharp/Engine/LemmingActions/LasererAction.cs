using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using System;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class LasererAction : ILemmingAction
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

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "laserer";
    public int NumberOfAnimationFrames => NumberOfLasererAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is LasererAction;
    public override bool Equals(object? obj) => obj is LasererAction;
    public override int GetHashCode() => nameof(LasererAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        if (!Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
        {
            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, false);
            return true;
        }

        var dx = lemming.FacingDirection.DeltaX;
        var target = lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 5);

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

        lemming.LaserHitPoint = target;

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
                CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, false);
            }
        }

        return true;
    }

    private LaserHitType CheckForHit(
        LevelPosition target,
        IOrientation orientation,
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
            var checkPoint = orientation.Move(target, offsetChecks[i]);

            var pixel = Terrain.GetPixelData(checkPoint);

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

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}