using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public static readonly StackerAction Instance = new();

    private StackerAction()
        : base(
            EngineConstants.StackerActionId,
            EngineConstants.StackerActionName,
            EngineConstants.StackerActionSpriteFileName,
            EngineConstants.StackerAnimationFrames,
            EngineConstants.MaxStackerPhysicsFrames,
            EngineConstants.NonPermanentSkillPriority)
    {
    }

    [SkipLocalsInit]
    public override bool UpdateLemming(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new LevelRegion(
            lemming.Orientation.MoveDown(lemming.LevelPosition, 1),
            lemming.Orientation.Move(lemming.LevelPosition, lemming.FacingDirection.DeltaX * 3, 1 + EngineConstants.NumberOfStackerBricks));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearRegion);

        if (lemming.PhysicsFrame == EngineConstants.StackerAnimationFrames - 1)
        {
            lemming.PlacedBrick = LayStackBrick(in gadgetsNearRegion, lemming);
            return true;
        }

        if (lemming.PhysicsFrame != 0)
            return true;

        lemming.NumberOfBricksLeft--;

        if (lemming.NumberOfBricksLeft < EngineConstants.NumberOfRemainingBricksToPlaySound)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position); ??
        }

        if (!lemming.PlacedBrick)
        {
            // Relax the check on the first brick
            // for details see http://www.lemmingsforums.net/index.php?topic=2862.0
            if (lemming.NumberOfBricksLeft < EngineConstants.NumberOfStackerBricks - 1 ||
                !MayPlaceNextBrick(in gadgetsNearRegion, lemming))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            return true;
        }

        if (lemming.NumberOfBricksLeft != 0)
            return true;

        ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private static bool MayPlaceNextBrick(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, 1 + EngineConstants.NumberOfStackerBricks - lemming.NumberOfBricksLeft);

        var dx = lemming.FacingDirection.DeltaX;

        return !(PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveRight(brickPosition, dx)) &&
                 PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveRight(brickPosition, dx * 2)) &&
                 PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveRight(brickPosition, dx * 3)));
    }

    private static bool LayStackBrick(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.StackLow ? -1 : 0;
        var brickPosition = orientation.Move(lemming.LevelPosition, dx, 1 + EngineConstants.NumberOfStackerBricks + dy - lemming.NumberOfBricksLeft);

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

        lemming.NumberOfBricksLeft = EngineConstants.NumberOfStackerBricks;
    }
}