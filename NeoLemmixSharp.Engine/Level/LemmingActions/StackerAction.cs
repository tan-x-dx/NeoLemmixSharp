using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public static readonly StackerAction Instance = new();

    private StackerAction()
        : base(
            LevelConstants.StackerActionId,
            LevelConstants.StackerActionName,
            LevelConstants.StackerAnimationFrames,
            LevelConstants.MaxStackerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var gadgetTestRegion = new LevelPositionPair(
            lemming.Orientation.MoveDown(lemming.LevelPosition, 1),
            lemming.Orientation.Move(lemming.LevelPosition, lemming.FacingDirection.DeltaX * 3, 1 + LevelConstants.NumberOfStackerBricks));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        if (lemming.PhysicsFrame == LevelConstants.StackerAnimationFrames - 1)
        {
            lemming.PlacedBrick = LayStackBrick(in gadgetsNearRegion, lemming);
        }
        else if (lemming.PhysicsFrame == 0)
        {
            lemming.NumberOfBricksLeft--;

            if (lemming.NumberOfBricksLeft < LevelConstants.NumberOfRemainingBricksToPlaySound)
            {
                // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position); ??
            }

            if (!lemming.PlacedBrick)
            {
                // Relax the check on the first brick
                // for details see http://www.lemmingsforums.net/index.php?topic=2862.0
                if (lemming.NumberOfBricksLeft < LevelConstants.NumberOfStackerBricks - 1 ||
                    !MayPlaceNextBrick(in gadgetsNearRegion, lemming))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
                }
            }
            else if (lemming.NumberOfBricksLeft == 0)
            {
                ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private static bool MayPlaceNextBrick(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, 1 + LevelConstants.NumberOfStackerBricks - lemming.NumberOfBricksLeft);

        var dx = lemming.FacingDirection.DeltaX;

        return !(PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveRight(brickPosition, dx)) &&
                 PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveRight(brickPosition, dx * 2)) &&
                 PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveRight(brickPosition, dx * 3)));
    }

    private static bool LayStackBrick(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.StackLow ? -1 : 0;
        var brickPosition = orientation.Move(lemming.LevelPosition, dx, 1 + LevelConstants.NumberOfStackerBricks + dy - lemming.NumberOfBricksLeft);

        var result = false;

        for (var i = 0; i < 3; i++)
        {
            if (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, brickPosition))
            {
                terrainManager.SetSolidPixel(brickPosition, Color.Magenta);
                result = true;
            }

            brickPosition = orientation.MoveRight(brickPosition, dx);
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = LevelConstants.NumberOfStackerBricks;
    }
}