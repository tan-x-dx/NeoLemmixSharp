using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class JumperAction : LemmingAction
{
    private const int JumperPositionCount = 6;
    private const int JumperArcFrames = 13;

    public static JumperAction Instance { get; } = new();

    private readonly Dictionary<Lemming, LevelPosition[]> _jumperPositionLookup = new();

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
        throw new NotImplementedException();
    }

    public ReadOnlySpan<LevelPosition> TryGetJumperPositions(Lemming lemming)
    {
        return _jumperPositionLookup.TryGetValue(lemming, out var array)
            ? new ReadOnlySpan<LevelPosition>(array)
            : ReadOnlySpan<LevelPosition>.Empty;
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

        _jumperPositionLookup.TryAdd(lemming, new LevelPosition[JumperPositionCount]);

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    private void DoGadgetChecks(Lemming lemming)
    {
        var gadgetsAtLemmingPosition = LevelConstants.GadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

        foreach (var gadget in gadgetsAtLemmingPosition)
        {
        }
    }
}

/*
function TLemmingGame.HandleJumping(L: TLemming): Boolean;
const
  JUMPER_ARC_FRAMES = 13;

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

  function MakeJumpMovement: Boolean;
  var
    Pattern: TJumpPattern;
    PatternIndex: Integer;

    FirstStepSpecialHandling: Boolean;

    i: Integer;
    n: Integer;

    CheckX: Integer;
  begin
    Result := false;

    case L.LemJumpProgress of
      0..1: PatternIndex := 0;
      2..3: PatternIndex := 1;
      4..8: PatternIndex := L.LemJumpProgress - 2;
      9..10: PatternIndex := 7;
      11..12: PatternIndex := 8;
      else Exit;
    end;

    Pattern := JUMP_PATTERNS[PatternIndex];
    FillChar(L.LemJumpPositions, SizeOf(L.LemJumpPositions), $FF);

    FirstStepSpecialHandling := (L.LemJumpProgress = 0);

    for i := 0 to 5 do
    begin
      L.LemJumpPositions[i, 0] := L.LemX;
      L.LemJumpPositions[i, 1] := L.LemY;

      if (Pattern[i][0] = 0) and (Pattern[i][1] = 0) then Break;

      if (Pattern[i][0] <> 0) then // Wall check
      begin
        CheckX := L.LemX + L.LemDX;
        if HasPixelAt(CheckX, L.LemY) then
        begin
          for n := 1 to 8 do
          begin
            if not HasPixelAt(CheckX, L.LemY - n) then
            begin
              if n <= 2 then
              begin
                L.LemX := CheckX;
                L.LemY := L.LemY - n + 1;
                fLemNextAction := baWalking;
              end else if n <= 5 then begin
                L.LemX := CheckX;
                L.LemY := L.LemY - n + 5;
                fLemNextAction := baHoisting;
                fLemJumpToHoistAdvance := true;
              end else begin
                L.LemX := CheckX;
                L.LemY := L.LemY - n + 8;
                fLemNextAction := baHoisting;
              end;

              Exit;
            end;

            if ((n = 5) and not (L.LemIsClimber)) or (n = 7) then
            begin
              if L.LemIsClimber then
              begin
                L.LemX := CheckX;
                fLemNextAction := baClimbing;
              end else begin
                if L.LemIsSlider then
                begin
                  Inc(L.LemX, L.LemDX);
                  fLemNextAction := baSliding;
                end else begin
                  L.LemDX := -L.LemDX;
                  fLemNextAction := baFalling;
                end;
              end;
              Exit;
            end;
          end;
        end;
      end;

      if (Pattern[i][1] < 0) then // Head check
      begin
        for n := 1 to 9 do
        begin
          if (n = 1) and FirstStepSpecialHandling then
            Continue;
        
          if HasPixelAt(L.LemX, L.LemY - n) then
          begin
            fLemNextAction := baFalling;
            Exit;
          end;
        end;
      end;

      L.LemX := L.LemX + (Pattern[i][0] * L.LemDX);
      L.LemY := L.LemY + Pattern[i][1];

      DoJumperTriggerChecks;

      if FirstStepSpecialHandling then
        FirstStepSpecialHandling := false
      else if HasPixelAt(L.LemX, L.LemY) then // Foot check
      begin
        fLemNextAction := baWalking;
        Exit;
      end;
    end;

    Result := true;
  end;
begin
  if MakeJumpMovement then
  begin
    Inc(L.LemJumpProgress);
    if (L.LemJumpProgress >= 8) and (L.LemIsGlider) then
      fLemNextAction := baGliding
    else if L.LemJumpProgress = JUMPER_ARC_FRAMES then
      fLemNextAction := baWalking;
  end;

  Result := true;
end;

*/