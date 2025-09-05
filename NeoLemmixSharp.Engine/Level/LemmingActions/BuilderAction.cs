using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BuilderAction : LemmingAction
{
    public static readonly BuilderAction Instance = new();

    private BuilderAction()
        : base(
            LemmingActionConstants.BuilderActionId,
            LemmingActionConstants.BuilderActionName,
            LemmingActionConstants.BuilderActionSpriteFileName,
            LemmingActionConstants.BuilderAnimationFrames,
            LemmingActionConstants.MaxBuilderPhysicsFrames,
            LemmingActionConstants.NonPermanentSkillPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.Data.PhysicsFrame == 9)
        {
            LayBrick(lemming);

            return true;
        }

        if (lemming.Data.PhysicsFrame == 10 &&
            lemming.Data.NumberOfBricksLeft <= EngineConstants.NumberOfRemainingBricksToPlaySound)
        {
            // play sound/make visual cue
            return true;
        }

        if (lemming.Data.PhysicsFrame != 0)
            return true;

        BuilderFrame0(lemming, in gadgetsNearLemming);
        lemming.Data.ConstructivePositionFreeze = false;

        return true;
    }

    private static void BuilderFrame0(
        Lemming lemming,
        in GadgetEnumerable gadgetsNearLemming)
    {
        lemming.Data.NumberOfBricksLeft--;

        var orientation = lemming.Data.Orientation;
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;
        var dx = lemming.Data.FacingDirection.DeltaX;

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 2)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx * 2, 2)) ||
            (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx * 2, 10)) &&
             lemming.Data.NumberOfBricksLeft > 0))
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx, 1);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.Data.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx * 2, 1);
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 2)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 3)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx * 2, 10)) &&
             lemming.Data.NumberOfBricksLeft > 0))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (lemming.Data.NumberOfBricksLeft == 0)
        {
            ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.Data.NumberOfBricksLeft = EngineConstants.NumberOfBuilderBricks;
        lemming.Data.ConstructivePositionFreeze = false;
    }

    public static void LayBrick(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Data.Orientation;
        var dx = lemming.Data.FacingDirection.DeltaX;
        var dy = lemming.CurrentAction == Instance
            ? 1
            : 0;

        var brickPosition = lemming.Data.AnchorPosition;
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
