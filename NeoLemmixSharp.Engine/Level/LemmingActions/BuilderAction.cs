using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class BuilderAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.PhysicsFrame == 9)
        {
            LayBrick(lemming);

            return true;
        }

        if (lemming.PhysicsFrame == 10 &&
            lemming.NumberOfBricksLeft <= EngineConstants.NumberOfRemainingBricksToPlaySound)
        {
            // play sound/make visual cue
            return true;
        }

        if (lemming.PhysicsFrame != 0)
            return true;

        BuilderFrame0(lemming, in gadgetsNearLemming);
        lemming.ConstructivePositionFreeze = false;

        return true;
    }

    private static void BuilderFrame0(
        Lemming lemming,
        in GadgetEnumerable gadgetsNearLemming)
    {
        lemming.NumberOfBricksLeft--;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(dx, 2))))
        {
            WalkerAction.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(dx, 3))) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(dx * 2, 2))) ||
            (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(dx * 2, 10))) &&
             lemming.NumberOfBricksLeft > 0))
        {
            lemmingPosition = orientation.Move(lemmingPosition, new(dx, 1));
            WalkerAction.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.Move(lemmingPosition, new(dx * 2, 1));
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 2)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 3)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(dx, 3))) ||
            (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(dx * 2, 10))) &&
             lemming.NumberOfBricksLeft > 0))
        {
            WalkerAction.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (lemming.NumberOfBricksLeft == 0)
        {
            ShruggerAction.TransitionLemmingToAction(lemming, false);
        }
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LemmingActionType.BuilderAction.DoMainTransitionActions(lemming, turnAround);

        lemming.NumberOfBricksLeft = EngineConstants.NumberOfBuilderBricks;
        lemming.ConstructivePositionFreeze = false;
    }

    public static void LayBrick(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.CurrentActionType == LemmingActionType.BuilderAction
            ? 1
            : 0;

        var brickPosition = lemming.AnchorPosition;
        brickPosition = orientation.MoveUp(brickPosition, dy);
        terrainManager.SetSolidPixel(brickPosition, Color.Magenta);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, Color.Magenta);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, Color.Magenta);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, Color.Magenta);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, Color.Magenta);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, Color.Magenta);
    }
}
