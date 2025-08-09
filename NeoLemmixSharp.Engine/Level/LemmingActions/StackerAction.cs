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
            EngineConstants.NonPermanentSkillPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.PhysicsFrame == LemmingActionConstants.StackerAnimationFrames - 1)
        {
            lemming.PlacedBrick = LayStackBrick(in gadgetsNearLemming, lemming);
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
                !MayPlaceNextBrick(in gadgetsNearLemming, lemming))
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

    private static bool MayPlaceNextBrick(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var brickPosition = lemming.AnchorPosition;
        brickPosition = orientation.MoveUp(brickPosition, 1 + EngineConstants.NumberOfStackerBricks - lemming.NumberOfBricksLeft);

        var dx = lemming.FacingDirection.DeltaX;

        return !(PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(brickPosition, dx)) &&
                 PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(brickPosition, dx * 2)) &&
                 PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(brickPosition, dx * 3)));
    }

    private static bool LayStackBrick(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.StackLow ? -1 : 0;
        var brickPosition = orientation.Move(lemming.AnchorPosition, dx, 1 + EngineConstants.NumberOfStackerBricks + dy - lemming.NumberOfBricksLeft);

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

        lemming.NumberOfBricksLeft = EngineConstants.NumberOfStackerBricks;
    }
}
