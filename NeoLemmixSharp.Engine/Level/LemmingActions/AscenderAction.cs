using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class AscenderAction : LemmingAction
{
    public static readonly AscenderAction Instance = new();

    private AscenderAction()
        : base(
            LevelConstants.AscenderActionId,
            LevelConstants.AscenderActionName,
            LevelConstants.AscenderActionSpriteFileName,
            LevelConstants.AscenderAnimationFrames,
            LevelConstants.MaxAscenderPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    [SkipLocalsInit]
    public override bool UpdateLemming(Lemming lemming)
    {
        ref var lemmingPosition = ref lemming.LevelPosition;
        var orientation = lemming.Orientation;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new LevelRegion(
            lemmingPosition,
            orientation.MoveUp(lemmingPosition, 2));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearRegion);

        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            dy++;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1IsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 1));
        var pixel2IsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 2));

        if (dy < 2 &&
            !pixel1IsSolid)
        {
            lemming.SetNextAction(WalkerAction.Instance);
            return true;
        }

        if ((lemming.AscenderProgress == 4 &&
             pixel1IsSolid &&
             pixel2IsSolid) ||
            (lemming.AscenderProgress >= 5 &&
             pixel1IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.LevelPosition = orientation.MoveLeft(lemmingPosition, dx);
            FallerAction.Instance.TransitionLemmingToAction(lemming, true);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 2;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.AscenderProgress = 0;
    }
}