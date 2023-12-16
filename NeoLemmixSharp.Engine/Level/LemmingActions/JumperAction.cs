using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class JumperAction : LemmingAction
{
    public const int JumperPositionCount = 6;
    private const int JumperArcFrames = 13;

    public static readonly JumperAction Instance = new();

    private readonly LevelPosition[] _jumpPositions =
    {
        new(0, -1), new(0, -1), new(1, 0), new(0, -1), new(0, -1), new(1, 0), // occurs twice
        new(0, -1), new(1, 0), new(0, -1), new(1, 0), new(0, -1), new(1, 0), // occurs twice
        new(0, -1), new(1, 0), new(0, -1), new(1, 0), new(1, 0), new(0, 0),
        new(0, -1), new(1, 0), new(1, 0), new(0, -1), new(1, 0), new(0, 0),
        new(1, 0), new(1, 0), new(1, 0), new(1, 0), new(0, 0), new(0, 0),
        new(1, 0), new(0, 1), new(1, 0), new(1, 0), new(0, 1), new(0, 0),
        new(1, 0), new(1, 0), new(0, 1), new(1, 0), new(0, 1), new(0, 0),
        new(1, 0), new(0, 1), new(1, 0), new(0, 1), new(1, 0), new(0, 1), // occurs twice
        new(1, 0), new(0, 1), new(0, 1), new(1, 0), new(0, 1), new(0, 1) // occurs twice
    };

    private JumperAction()
    {
    }

    public override int Id => LevelConstants.JumperActionId;
    public override string LemmingActionName => "jumper";
    public override int NumberOfAnimationFrames => LevelConstants.JumperAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!MakeJumpMovement(lemming))
            return true;

        lemming.JumpProgress++;

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

    private bool MakeJumpMovement(Lemming lemming)
    {
        var patternIndex = GetPatternIndex(lemming);
        if (patternIndex < 0)
            return false;

        var firstStepSpecialHandling = lemming.JumpProgress == 0;
        var patternSpan = new ReadOnlySpan<LevelPosition>(_jumpPositions, patternIndex * JumperPositionCount, JumperPositionCount);
        var lemmingJumpPatterns = lemming.GetJumperPositions();

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        for (var i = 0; i < JumperPositionCount; i++)
        {
            lemmingJumpPatterns[i] = lemmingPosition;

            var position = patternSpan[i];

            if (position.X == 0 && position.Y == 0)
                break;

            if (position.X != 0) // Wall check
            {
                var hitWall = DoWallCheck(lemming);
                if (hitWall)
                    return false;
            }

            if (position.Y < 0) // Head check
            {
                var hitHead = DoHeadCheck(lemming, firstStepSpecialHandling);
                if (hitHead)
                    return false;
            }

            lemmingPosition = orientation.Move(lemmingPosition, patternSpan[i]);

            DoJumperTriggerChecks(lemming);

            if (firstStepSpecialHandling)
            {
                firstStepSpecialHandling = false;
            }
            else if (LevelConstants.TerrainManager.PixelIsSolidToLemming(lemming, lemmingPosition)) // Foot check
            {
                lemming.SetNextAction(WalkerAction.Instance);
                return false;
            }
        }

        return true;
    }

    private static int GetPatternIndex(Lemming lemming)
    {
        var jumpProgress = lemming.JumpProgress;

        return jumpProgress switch
        {
            0 => 0,
            1 => 0,
            2 => 1,
            3 => 1,
            >= 4 and <= 8 => jumpProgress - 2,
            9 => 7,
            10 => 7,
            11 => 8,
            12 => 8,
            _ => -1
        };
    }

    private static bool DoWallCheck(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var checkPosition = orientation.MoveRight(lemmingPosition, dx);
        if (!LevelConstants.TerrainManager.PixelIsSolidToLemming(lemming, checkPosition))
            return false;

        for (var n = 1; n < 9; n++)
        {
            var checkPosition2 = orientation.MoveUp(checkPosition, n);
            if (!LevelConstants.TerrainManager.PixelIsSolidToLemming(lemming, checkPosition2))
            {
                switch (n)
                {
                    case <= 2:
                        lemmingPosition = orientation.MoveUp(checkPosition, n - 1);
                        lemming.SetNextAction(WalkerAction.Instance);
                        break;

                    case <= 5:
                        lemmingPosition = orientation.MoveUp(checkPosition, n - 5);
                        lemming.SetNextAction(HoisterAction.Instance);
                        break;

                    default:
                        lemmingPosition = orientation.MoveUp(checkPosition, n - 8);
                        lemming.SetNextAction(HoisterAction.Instance);
                        break;
                }

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
        Lemming lemming,
        bool firstStepSpecialHandling)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        for (var n = 1; n < 10; n++)
        {
            if (n == 1 && firstStepSpecialHandling)
                continue;

            var checkPosition = orientation.MoveUp(lemmingPosition, n);
            if (!LevelConstants.TerrainManager.PixelIsSolidToLemming(lemming, checkPosition))
                continue;

            lemming.SetNextAction(FallerAction.Instance);
            return true;
        }

        return false;
    }

    private void DoJumperTriggerChecks(Lemming lemming)
    {
        var gadgetsAtLemmingPosition = LevelConstants.GadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

        foreach (var gadget in gadgetsAtLemmingPosition)
        {
        }
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