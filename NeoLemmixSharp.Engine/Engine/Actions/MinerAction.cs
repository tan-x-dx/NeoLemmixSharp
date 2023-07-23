﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class MinerAction : LemmingAction
{
    public const int NumberOfMinerAnimationFrames = 24;

    public static MinerAction Instance { get; } = new();

    private MinerAction()
    {
    }

    public override int Id => 6;
    public override string LemmingActionName => "miner";
    public override int NumberOfAnimationFrames => NumberOfMinerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (lemming.AnimationFrame == 1 ||
            lemming.AnimationFrame == 2)
        {
            TerrainMasks.ApplyMinerMask(
                lemming,
                new LevelPosition(0, 0),
                lemming.AnimationFrame - 1);
            return true;
        }

        if (lemming.AnimationFrame != 3 &&
            lemming.AnimationFrame != 15)
            return true;

        if (lemming.IsSlider && WalkerAction.LemmingCanDehoist(lemming, false))
        {
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        lemmingPosition = orientation.Move(lemmingPosition, dx + dx, -1);
        lemming.LevelPosition = lemmingPosition;

        if (lemming.IsSlider && WalkerAction.LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        // Note that all if-checks are relative to the end position!

        // Lem cannot go down, so turn; see http://www.lemmingsforums.net/index.php?topic=2547.0
        if (Terrain.PixelIsIndestructibleToLemming(orientation.Move(lemmingPosition, -dx, -1), lemming) &&
            Terrain.PixelIsIndestructibleToLemming(orientation.MoveDown(lemmingPosition, 1), lemming))
        {
            var lemmingPosition0 = orientation.MoveDown(lemmingPosition, 1);
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx + dx);
            lemming.LevelPosition = lemmingPosition;
            TurnMinerAround(lemming, lemmingPosition0);
            return true;
        }

        // This first check is only relevant during the very first cycle.
        // Otherwise the pixel was already checked in frame 15 of the previous cycle
        if (lemming.AnimationFrame == 3 && Terrain.PixelIsIndestructibleToLemming(orientation.Move(lemmingPosition, -dx, 2), lemming))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx + dx);
            lemming.LevelPosition = lemmingPosition;
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));

            return true;
        }

        // Do we really want the to check the second HasPixel during frame 3 ????
        if (!Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, 1), lemming) &&
            !Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, 0), lemming) &&
            !Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, -1), lemming))
        {
            lemmingPosition = orientation.Move(lemmingPosition, -dx, -1);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            lemming.DistanceFallen++;
            return true;
        }

        if (Terrain.PixelIsIndestructibleToLemming(orientation.MoveDown(lemmingPosition, 2), lemming))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));
            return true;
        }

        if (!Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (Terrain.PixelIsIndestructibleToLemming(orientation.Move(lemmingPosition, dx, 2), lemming))
        {
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));

            return true;
        }

        if (!Terrain.PixelIsIndestructibleToLemming(lemmingPosition, lemming))
            return true;

        TurnMinerAround(lemming, lemmingPosition);

        return true;
    }

    private static void TurnMinerAround(
        Lemming lemming,
        LevelPosition checkPosition)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (Terrain.PixelIsIndestructibleToLemming(checkPosition, lemming))
        {
            // CueSoundEffect(SFX_HITS_STEEL, L.Position);
        }

        // Independently of checkPosition this check is always made at Lem position
        // No longer check at Lem position, due to http://www.lemmingsforums.net/index.php?topic=2547.0

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);

        if (Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
        {
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);// turn around as well
        }
    }
}
