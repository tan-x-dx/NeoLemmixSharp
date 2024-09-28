using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BlockerAction : LemmingAction
{
    public static readonly BlockerAction Instance = new();

    private BlockerAction()
        : base(
            LevelConstants.BlockerActionId,
            LevelConstants.BlockerActionName,
            LevelConstants.BlockerActionSpriteFileName,
            LevelConstants.BlockerAnimationFrames,
            LevelConstants.MaxBlockerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        LevelScreen.GadgetManager.GetAllGadgetsForPosition(lemming.LevelPosition, out var gadgetsNearRegion);

        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemming.LevelPosition))
            return true;

        LevelScreen.LemmingManager.DeregisterBlocker(lemming);
        FallerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        LevelScreen.LemmingManager.RegisterBlocker(lemming);
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -7;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => -3;

    public static void DoBlockerCheck(Lemming lemming)
    {
        if (CheckPixelForBlocker(lemming, lemming.LevelPosition))
            return;
        CheckPixelForBlocker(lemming, lemming.FootPosition);
    }

    private static bool CheckPixelForBlocker(Lemming lemming, LevelPosition testPosition)
    {
        var pixel = LevelScreen.TerrainManager.GetBlockerData(testPosition);
        if (pixel == PixelType.Empty)
            return false;

        var pixelBlockerOrientation = PixelTypeHelpers.GetOrientationFromBlockerMask(pixel);

        if (!lemming.Orientation.IsPerpendicularTo(pixelBlockerOrientation))
            return false;

        return HandleBlocker(lemming, pixelBlockerOrientation);
    }

    private static bool HandleBlocker(Lemming lemming, Orientation pixelBlockerOrientation)
    {
        var lemmingFacingDirection = lemming.FacingDirection;
        var lemmingFacingDirectionAsOrientation = lemmingFacingDirection.ConvertToRelativeOrientation(lemming.Orientation);

        if (lemmingFacingDirectionAsOrientation != Orientation.GetOpposite(pixelBlockerOrientation))
            return false;

        ForceLemmingDirection(lemming, lemmingFacingDirection.GetOpposite());
        return true;
    }

    public static void ForceLemmingDirection(Lemming lemming, FacingDirection forcedFacingDirection)
    {
        if (lemming.FacingDirection == forcedFacingDirection)
            return;

        lemming.SetFacingDirection(forcedFacingDirection);

        var dx = forcedFacingDirection.DeltaX;

        var currentAction = lemming.CurrentAction;

        // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
        if (currentAction == MinerAction.Instance)
        {
            int mineDx;
            int mineDy;

            if (lemming.PhysicsFrame == 2)
            {
                mineDx = 0;
                mineDy = 0;
            }
            else if (lemming.PhysicsFrame >= 3 && lemming.PhysicsFrame < 15)
            {
                mineDx = -2 * dx;
                mineDy = -1;
            }
            else
            {
                return;
            }

            TerrainMasks.ApplyMinerMask(lemming, 1, mineDx, mineDy);

            return;
        }

        // Required for turned builders not to walk into air
        // For platformers, see http://www.lemmingsforums.net/index.php?topic=2530.0
        if ((currentAction == BuilderAction.Instance ||
             currentAction == PlatformerAction.Instance) &&
            lemming.PhysicsFrame >= 9)
        {
            BuilderAction.LayBrick(lemming);
            return;
        }

        if (currentAction != ClimberAction.Instance &&
            currentAction != SliderAction.Instance &&
            currentAction != DehoisterAction.Instance)
            return;

        // Don't move below original position
        var dy = lemming.IsStartingAction
            ? 0
            : 1;

        // Move out of the wall
        lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, dy);

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    public static void SetBlockerField(Lemming lemming, bool set)
    {
        var previousPosition = lemming.PreviousLevelPosition;
        var currentPosition = lemming.LevelPosition;

        if (previousPosition == currentPosition)
        {
            if (set)
                return;

            UpdateBlockerField(lemming, currentPosition, false);
            return;
        }

        UpdateBlockerField(lemming, previousPosition, false);
        UpdateBlockerField(lemming, currentPosition, set);
    }

    private static void UpdateBlockerField(Lemming lemming, LevelPosition levelPosition, bool set)
    {
        var orientation = lemming.Orientation;
        var rightArmPixelType = set ? GetRightArmPixelType(orientation) : PixelType.BlockerInverseMask;
        var leftArmPixelType = set ? GetLeftArmPixelType(orientation) : PixelType.BlockerInverseMask;

        var moveDelta = lemming.FacingDirection.Id - 1; // Fixes off-by-one errors between left/right
        levelPosition = orientation.MoveRight(levelPosition, moveDelta);

        var terrainManager = LevelScreen.TerrainManager;

        for (var y = -3; y < 7; y++)
        {
            var workPosition = orientation.Move(levelPosition, -5, y);
            terrainManager.SetBlockerMaskPixel(workPosition, leftArmPixelType, set);
            workPosition = orientation.Move(levelPosition, -4, y);
            terrainManager.SetBlockerMaskPixel(workPosition, leftArmPixelType, set);
            workPosition = orientation.Move(levelPosition, -3, y);
            terrainManager.SetBlockerMaskPixel(workPosition, leftArmPixelType, set);
            workPosition = orientation.Move(levelPosition, -2, y);
            terrainManager.SetBlockerMaskPixel(workPosition, leftArmPixelType, set);

            workPosition = orientation.Move(levelPosition, 3, y);
            terrainManager.SetBlockerMaskPixel(workPosition, rightArmPixelType, set);
            workPosition = orientation.Move(levelPosition, 4, y);
            terrainManager.SetBlockerMaskPixel(workPosition, rightArmPixelType, set);
            workPosition = orientation.Move(levelPosition, 5, y);
            terrainManager.SetBlockerMaskPixel(workPosition, rightArmPixelType, set);
            workPosition = orientation.Move(levelPosition, 6, y);
            terrainManager.SetBlockerMaskPixel(workPosition, rightArmPixelType, set);
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PixelType GetRightArmPixelType(Orientation orientation)
    {
        orientation = Orientation.RotateCounterClockwise(orientation);
        return (PixelType)(1 << (PixelTypeHelpers.PixelTypeBlockerShiftOffset + orientation.RotNum));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PixelType GetLeftArmPixelType(Orientation orientation)
    {
        orientation = Orientation.RotateClockwise(orientation);
        return (PixelType)(1 << (PixelTypeHelpers.PixelTypeBlockerShiftOffset + orientation.RotNum));
    }
}