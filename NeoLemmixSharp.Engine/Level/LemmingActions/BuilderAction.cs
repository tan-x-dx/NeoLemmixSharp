using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BuilderAction : LemmingAction
{
    public static readonly BuilderAction Instance = new();

    private BuilderAction()
        : base(
            LevelConstants.BuilderActionId,
            LevelConstants.BuilderActionName,
            LevelConstants.BuilderActionSpriteFileName,
            LevelConstants.BuilderAnimationFrames,
            LevelConstants.MaxBuilderPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.PhysicsFrame == 9)
        {
            LayBrick(lemming);

            return true;
        }

        if (lemming.PhysicsFrame == 10 &&
            lemming.NumberOfBricksLeft <= LevelConstants.NumberOfRemainingBricksToPlaySound)
        {
            // play sound/make visual cue
            return true;
        }

        if (lemming.PhysicsFrame != 0)
            return true;

        BuilderFrame0(lemming);
        lemming.ConstructivePositionFreeze = false;

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;

    private static void BuilderFrame0(Lemming lemming)
    {
        lemming.NumberOfBricksLeft--;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx * 3, 10));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 2)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx * 2, 2)) ||
            (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx * 2, 10)) &&
             lemming.NumberOfBricksLeft > 0))
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx, 1);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx * 2, 1);
        }

        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 2)) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 3)) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx * 2, 10)) &&
             lemming.NumberOfBricksLeft > 0))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (lemming.NumberOfBricksLeft == 0)
        {
            ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = LevelConstants.NumberOfBuilderBricks;
        lemming.ConstructivePositionFreeze = false;
    }

    public static void LayBrick(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.CurrentAction == Instance
            ? 1
            : 0;

        var brickPosition = lemming.LevelPosition;
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