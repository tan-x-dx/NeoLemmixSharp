using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.InteropServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class JumperAction
{
    public const int JumperPositionCount = 6;
    private const int JumperArcFrames = 13;

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

    private static ReadOnlySpan<Point> JumpPositionsFor(Lemming lemming)
    {
        var jumpProgress = lemming.JumpProgress;

        var patternIndex = jumpProgress switch
        {
            0 or 1 => 0,
            2 or 3 => 1,
            >= 4 and <= 8 => jumpProgress - 2,
            9 or 10 => 7,
            11 or 12 => 8,
            _ => -1
        };

        if (patternIndex < 0)
            return ReadOnlySpan<Point>.Empty;

        return MemoryMarshal.Cast<int, Point>(RawLevelPositions).SliceUnsafe(
            patternIndex * JumperPositionCount,
            JumperPositionCount);
    }

    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (!MakeJumpMovement(lemming, in gadgetsNearLemming))
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

        if (lemming.JumpProgress >= 8 && lemming.IsGlider)
        {
            lemming.SetNextActionType(LemmingActionType.GliderAction);
            return true;
        }

        if (lemming.JumpProgress == JumperArcFrames)
        {
            lemming.SetNextActionType(LemmingActionType.WalkerAction);
        }

        return true;
    }

    private static bool MakeJumpMovement(
        Lemming lemming,
        in GadgetEnumerable gadgetsNearLemming)
    {
        var jumpPositionPatterns = JumpPositionsFor(lemming);
        if (jumpPositionPatterns.Length == 0)
            return false;

        var lemmingJumpPatterns = lemming.GetJumperPositions();

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

        for (var i = 0; i < JumperPositionCount; i++)
        {
            lemmingJumpPatterns.At(i) = lemmingPosition;

            var position = jumpPositionPatterns.At(i);

            if (position.X == 0 && position.Y == 0)
                break;

            if (position.X != 0) // Wall check
            {
                var hitWall = DoWallCheck(in gadgetsNearLemming, lemming);
                if (hitWall)
                    return false;
            }

            if (position.Y > 0) // Head check
            {
                var hitHead = DoHeadCheck(in gadgetsNearLemming, lemming, lemming.JumpProgress == 0);
                if (hitHead)
                    return false;
            }

            lemmingPosition = orientation.Move(lemmingPosition, dx * position.X, position.Y);

            DoJumperTriggerChecks(in gadgetsNearLemming);

            if (lemming.JumpProgress == 0 ||
                !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
                continue; // Foot check

            lemming.SetNextActionType(LemmingActionType.WalkerAction);
            return false;
        }

        return true;
    }

    private static bool DoWallCheck(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var checkPosition = orientation.MoveRight(lemmingPosition, dx);
        if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, checkPosition))
            return false;

        for (var n = 1; n < 9; n++)
        {
            var checkPosition2 = orientation.MoveUp(checkPosition, n);
            if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, checkPosition2))
            {
                int deltaY;
                LemmingActionType nextActionType;

                if (n <= 2)
                {
                    deltaY = n - 1;
                    nextActionType = LemmingActionType.WalkerAction;
                }
                else if (n <= 5)
                {
                    deltaY = n - 5;
                    nextActionType = LemmingActionType.HoisterAction;
                    lemming.JumpToHoistAdvance = true;
                }
                else
                {
                    deltaY = n - 8;
                    nextActionType = LemmingActionType.HoisterAction;
                }

                lemmingPosition = orientation.MoveUp(checkPosition, deltaY);
                lemming.SetNextActionType(nextActionType);

                return true;
            }

            var isClimber = lemming.IsClimber;
            if (n != 7 && (n != 5 || isClimber))
                continue;

            if (isClimber)
            {
                lemmingPosition = checkPosition;
                lemming.SetNextActionType(LemmingActionType.ClimberAction);
                return true;
            }

            if (lemming.IsSlider)
            {
                lemmingPosition = checkPosition;
                lemming.SetNextActionType(LemmingActionType.SliderAction);
                return true;
            }

            lemming.FacingDirection = lemming.FacingDirection.GetOpposite();
            lemming.SetNextActionType(LemmingActionType.FallerAction);

            return true;
        }

        return false;
    }

    private static bool DoHeadCheck(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming,
        bool firstStepSpecialHandling)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.AnchorPosition;

        var n = firstStepSpecialHandling
            ? 1
            : 0;

        n++;

        for (; n < 10; n++)
        {
            var checkPosition = orientation.MoveUp(lemmingPosition, n);
            if (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, checkPosition))
                continue;

            lemming.SetNextActionType(LemmingActionType.FallerAction);
            return true;
        }

        return false;
    }

    private static void DoJumperTriggerChecks(
        in GadgetEnumerable gadgetsNearLemming)
    {
        foreach (var gadget in gadgetsNearLemming)
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

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var currentActionType = lemming.CurrentActionType;
        if (currentActionType == LemmingActionType.ClimberAction ||
            currentActionType == LemmingActionType.SliderAction)
        {
            lemming.FacingDirection = lemming.FacingDirection.GetOpposite();
            lemming.AnchorPosition = lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX);
        }

        LemmingActionType.JumperAction.DoMainTransitionActions(lemming, turnAround);

        lemming.JumpProgress = 0;
    }
}
