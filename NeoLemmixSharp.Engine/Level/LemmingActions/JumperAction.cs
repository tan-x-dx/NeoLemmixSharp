using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class JumperAction : LemmingAction
{
    public const int JumperPositionCount = 6;
    private const int JumperArcFrames = 13;

    public static readonly JumperAction Instance = new();

    private static ReadOnlySpan<int> RawLevelPositions =>
    [
        0, 1,   0,  1,   1,  0,   0,  1,   0,  1,   1,  0,
        0, 1,   1,  0,   0,  1,   1,  0,   0,  1,   1,  0,
        0, 1,   1,  0,   0,  1,   1,  0,   1,  0,   0,  0,
        0, 1,   1,  0,   1,  0,   0,  1,   1,  0,   0,  0,
        1, 0,   1,  0,   1,  0,   1,  0,   0,  0,   0,  0,
        1, 0,   0, -1,   1,  0,   1,  0,   0, -1,   0,  0,
        1, 0,   1,  0,   0, -1,   1,  0,   0, -1,   0,  0,
        1, 0,   0, -1,   1,  0,   0, -1,   1,  0,   0, -1,
        1, 0,   0, -1,   0, -1,   1,  0,   0, -1,   0, -1
    ];

    private static ReadOnlySpan<LevelPosition> JumpPositionsFor(int patternIndex) => MemoryMarshal
        .Cast<int, LevelPosition>(RawLevelPositions)
        .Slice(patternIndex * JumperPositionCount, JumperPositionCount);

    private JumperAction()
        : base(
            LevelConstants.JumperActionId,
            LevelConstants.JumperActionName,
            LevelConstants.JumperActionSpriteFileName,
            LevelConstants.JumperAnimationFrames,
            LevelConstants.MaxJumperPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!MakeJumpMovement(lemming))
            return true;

        lemming.JumpProgress++;

        if (lemming.JumpProgress >= 0 && lemming.JumpProgress <= 5)
        {
            lemming.AnimationFrame = 0;
        }
        else if (lemming.JumpProgress == 6)
        {
            lemming.AnimationFrame = 1;
        }
        else
        {
            lemming.AnimationFrame = 2;
        }

        if (lemming.JumpProgress >= 8 && lemming.State.IsGlider)
        {
            lemming.SetNextAction(GliderAction.Instance);
            return true;
        }

        if (lemming.JumpProgress == JumperArcFrames)
        {
            lemming.SetNextAction(WalkerAction.Instance);
        }

        return true;
    }

    [SkipLocalsInit]
    private bool MakeJumpMovement(
        Lemming lemming)
    {
        var patternIndex = GetPatternIndex(lemming);
        if (patternIndex < 0)
            return false;

        var patternSpan = JumpPositionsFor(patternIndex);
        var lemmingJumpPatterns = lemming.GetJumperPositions();

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx, 12));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearRegion);

        for (var i = 0; i < JumperPositionCount; i++)
        {
            lemmingJumpPatterns[i] = lemmingPosition;

            var position = patternSpan[i];

            if (position.X == 0 && position.Y == 0)
                break;

            if (position.X != 0) // Wall check
            {
                var hitWall = DoWallCheck(in gadgetsNearRegion, lemming);
                if (hitWall)
                    return false;
            }

            if (position.Y > 0) // Head check
            {
                var hitHead = DoHeadCheck(in gadgetsNearRegion, lemming, lemming.JumpProgress == 0);
                if (hitHead)
                    return false;
            }

            lemmingPosition = orientation.Move(lemmingPosition, dx * position.X, position.Y);

            DoJumperTriggerChecks(in gadgetsNearRegion);

            if (lemming.JumpProgress == 0 ||
                !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
                continue; // Foot check

            lemming.SetNextAction(WalkerAction.Instance);
            return false;
        }

        return true;
    }

    private static int GetPatternIndex(Lemming lemming)
    {
        var jumpProgress = lemming.JumpProgress;

        return jumpProgress switch
        {
            0 or 1 => 0,
            2 or 3 => 1,
            >= 4 and <= 8 => jumpProgress - 2,
            9 or 10 => 7,
            11 or 12 => 8,
            _ => -1
        };
    }

    private static bool DoWallCheck(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var checkPosition = orientation.MoveRight(lemmingPosition, dx);
        if (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkPosition))
            return false;

        for (var n = 1; n < 9; n++)
        {
            var checkPosition2 = orientation.MoveUp(checkPosition, n);
            if (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkPosition2))
            {
                int deltaY;
                LemmingAction nextAction;

                if (n <= 2)
                {
                    deltaY = n - 1;
                    nextAction = WalkerAction.Instance;
                }
                else if (n <= 5)
                {
                    deltaY = n - 5;
                    nextAction = HoisterAction.Instance;
                    lemming.JumpToHoistAdvance = true;
                }
                else
                {
                    deltaY = n - 8;
                    nextAction = HoisterAction.Instance;
                }

                lemmingPosition = orientation.MoveUp(checkPosition, deltaY);
                lemming.SetNextAction(nextAction);

                return true;
            }

            var isClimber = lemming.State.IsClimber;
            if (n != 7 && (n != 5 || isClimber))
                continue;

            if (isClimber)
            {
                lemmingPosition = checkPosition;
                lemming.SetNextAction(ClimberAction.Instance);
                return true;
            }

            if (lemming.State.IsSlider)
            {
                lemmingPosition = checkPosition;
                lemming.SetNextAction(SliderAction.Instance);
                return true;
            }

            lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
            lemming.SetNextAction(FallerAction.Instance);

            return true;
        }

        return false;
    }

    private static bool DoHeadCheck(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming,
        bool firstStepSpecialHandling)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var n = firstStepSpecialHandling
            ? 2
            : 1;

        for (; n < 10; n++)
        {
            var checkPosition = orientation.MoveUp(lemmingPosition, n);
            if (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkPosition))
                continue;

            lemming.SetNextAction(FallerAction.Instance);
            return true;
        }

        return false;
    }

    private void DoJumperTriggerChecks(
        in GadgetSet gadgetsNearRegion)
    {
        foreach (var gadget in gadgetsNearRegion)
        {
        }
    }

    /*
      procedure DoJumperTriggerChecks;
      begin
        if not HasTriggerAt(L.LemX, L.LemY, trFlipper) then
          L.LemInFlipper := DOM_NOOBJECT
        else
          if HandleFlipper(L, L.LemX, L.LemY) then
            Exit;

        if HasTriggerAt(L.LemX, L.LemY, trZombie, L) and not L.LemIsZombie then
          RemoveLemming(L, RM_ZOMBIE);

        if HasTriggerAt(L.LemX, L.LemY, trForceLeft, L) then
          HandleForceField(L, -1)
        else if HasTriggerAt(L.LemX, L.LemY, trForceRight, L) then
          HandleForceField(L, 1);
      end;
    */

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -1;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
            lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX);
        }

        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.JumpProgress = 0;
    }
}