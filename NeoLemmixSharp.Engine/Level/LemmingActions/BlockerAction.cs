using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BlockerAction : LemmingAction
{
    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public override int Id => Global.BlockerActionId;
    public override string LemmingActionName => "blocker";
    public override int NumberOfAnimationFrames => Global.BlockerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => Global.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!Global.TerrainManager.PixelIsSolidToLemming(lemming, lemming.LevelPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        Global.LemmingManager.RegisterBlocker(lemming);

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -7;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;

    public static bool BlockerMatches(
        Lemming blocker,
        Lemming lemming,
        LevelPosition anchorPosition,
        LevelPosition footPosition)
    {
        Debug.Assert(blocker.CurrentAction == Instance);

        var blockerOrientation = blocker.Orientation;
        var lemmingOrientation = lemming.Orientation;

        if (blockerOrientation.IsPerpendicularTo(lemmingOrientation))
            return false;

        var lemmingFacingDirection = lemming.FacingDirection;
        // If the lemming is upside down compared to the blocker, it cares about the opposite hands.
        if (lemmingOrientation == blockerOrientation.GetOpposite())
        {
            lemmingFacingDirection = lemmingFacingDirection.GetOpposite();
        }

        LevelPosition topLeftDelta;
        LevelPosition bottomRightDelta;

        if (lemmingFacingDirection == RightFacingDirection.Instance)
        {
            topLeftDelta = GetLeftBlockingTopLeftDelta();
            bottomRightDelta = GetLeftBlockingBottomRightDelta();
        }
        else
        {
            topLeftDelta = GetRightBlockingTopLeftDelta();
            bottomRightDelta = GetRightBlockingBottomRightDelta();
        }

        var dxCorrection = 1 - blocker.FacingDirection.Id; // Fixes off-by-one errors with left/right positions
        var blockerLevelPosition = blocker.LevelPosition;

        var p1 = blockerOrientation.MoveWithoutNormalization(blockerLevelPosition, dxCorrection + topLeftDelta.X, topLeftDelta.Y);
        var p2 = blockerOrientation.MoveWithoutNormalization(blockerLevelPosition, dxCorrection + bottomRightDelta.X, bottomRightDelta.Y);
        var blockingRegion = new LevelPositionPair(p1, p2);

        return blockingRegion.Contains(anchorPosition) ||
               blockingRegion.Contains(footPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LevelPosition GetLeftBlockingTopLeftDelta() => new(-5, -6);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LevelPosition GetLeftBlockingBottomRightDelta() => new(-2, 3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LevelPosition GetRightBlockingTopLeftDelta() => new(3, -6);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LevelPosition GetRightBlockingBottomRightDelta() => new(6, 3);
}