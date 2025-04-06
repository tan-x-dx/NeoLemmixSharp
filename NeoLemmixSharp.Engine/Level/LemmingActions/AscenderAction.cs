using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class AscenderAction : LemmingAction
{
    public static readonly AscenderAction Instance = new();

    private AscenderAction()
        : base(
            EngineConstants.AscenderActionId,
            EngineConstants.AscenderActionName,
            EngineConstants.AscenderActionSpriteFileName,
            EngineConstants.AscenderAnimationFrames,
            EngineConstants.MaxAscenderPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var orientation = lemming.Orientation;

        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            dy++;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1IsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1));
        var pixel2IsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 2));

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
            lemming.AnchorPosition = orientation.MoveLeft(lemmingPosition, dx);
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