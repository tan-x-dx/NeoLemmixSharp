using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public static readonly StackerAction Instance = new();

    private StackerAction()
        : base(
            LemmingActionConstants.StackerActionId,
            LemmingActionConstants.StackerActionName,
            LemmingActionConstants.StackerActionSpriteFileName,
            LemmingActionConstants.StackerAnimationFrames,
            LemmingActionConstants.MaxStackerPhysicsFrames,
            LemmingActionConstants.NonPermanentSkillPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.Data.PhysicsFrame == LemmingActionConstants.StackerAnimationFrames - 1)
        {
            lemming.Data.PlacedBrick = LayStackBrick(in gadgetsNearLemming, lemming);
            return true;
        }

        if (lemming.Data.PhysicsFrame != 0)
            return true;

        lemming.Data.NumberOfBricksLeft--;

        if (lemming.Data.NumberOfBricksLeft < EngineConstants.NumberOfRemainingBricksToPlaySound)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position); ??
        }

        if (!lemming.Data.PlacedBrick)
        {
            // Relax the check on the first brick
            // for details see http://www.lemmingsforums.net/index.php?topic=2862.0
            if (lemming.Data.NumberOfBricksLeft < EngineConstants.NumberOfStackerBricks - 1 ||
                !MayPlaceNextBrick(in gadgetsNearLemming, lemming))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            return true;
        }

        if (lemming.Data.NumberOfBricksLeft != 0)
            return true;

        ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    private static bool MayPlaceNextBrick(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming)
    {
        var orientation = lemming.Data.Orientation;
        var brickPosition = lemming.Data.AnchorPosition;
        brickPosition = orientation.MoveUp(brickPosition, 1 + EngineConstants.NumberOfStackerBricks - lemming.Data.NumberOfBricksLeft);

        var dx = lemming.Data.FacingDirection.DeltaX;

        return !(PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(brickPosition, dx)) &&
                 PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(brickPosition, dx * 2)) &&
                 PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(brickPosition, dx * 3)));
    }

    private static bool LayStackBrick(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Data.Orientation;
        var dx = lemming.Data.FacingDirection.DeltaX;
        var dy = lemming.Data.StackLow ? -1 : 0;
        var brickPosition = orientation.Move(lemming.Data.AnchorPosition, dx, 1 + EngineConstants.NumberOfStackerBricks + dy - lemming.Data.NumberOfBricksLeft);

        var result = false;

        for (var i = 0; i < 3; i++)
        {
            if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, brickPosition))
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
        DoMainTransitionActions(lemming, turnAround);

        lemming.Data.NumberOfBricksLeft = EngineConstants.NumberOfStackerBricks;
    }
}
