namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class GliderAction : LemmingAction
{
    public const int NumberOfGliderAnimationFrames = 17;

    public static GliderAction Instance { get; } = new();

    private GliderAction()
    {
    }

    public override int Id => 15;
    public override string LemmingActionName => "glider";
    public override int NumberOfAnimationFrames => NumberOfGliderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    private static bool DoTurnAround(Lemming lemming, bool moveForwardFirst)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var currentPosition = lemming.LevelPosition;
        if (moveForwardFirst)
        {
            currentPosition = lemming.Orientation.MoveRight(currentPosition, dx);
        }

        var dy = 0;
        var result = false;
        do
        {
            // bug-fix for http://www.lemmingsforums.net/index.php?topic=2693
            if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(currentPosition, dy), lemming) &&
                Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(currentPosition, -dx, dy), lemming))
            {
                return true;
            }

            dy++;

        } while (dy <= 3 && Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(currentPosition, dy), lemming));
        /*
        repeat
      if HasPixelAt(CurLemX, L.LemY + Dy) and HasPixelAt(CurLemX - L.LemDx, L.LemY + Dy) then
      begin
        // Abort computation and let lemming turn around
        Result := True;
        Exit;
      end;
      Inc(Dy);
    until (Dy > 3) or (not HasPixelAt(CurLemX, L.LemY + Dy));
        */
        return dy > 3;
    }

    // Special behavior in 1-pixel wide shafts: Move one pixel down even when turning
    private static void CheckOnePixelShaft(Lemming lemming)
    {

    }

    private static bool HasConsecutivePixels(Lemming lemming)
    {
        // Check at LemY +1, +2, +3 for (a) solid terrain, or (b) a one-way field that will turn the lemming around
        var result = false;

        var dx = lemming.FacingDirection.DeltaX;


        return result;
    }
    /*
function TLemmingGame.HandleGliding(L: TLemming): Boolean;
var
  MaxFallDist, GroundDist: Integer;
  LemDy: Integer;
    
  procedure CheckOnePixelShaft(L: TLemming);
  var
    LemYDir: Integer;

    function HasConsecutivePixels: Boolean;
    var
      i: Integer;
      OneWayCheckType: TTriggerTypes;
    begin
      
      Result := false;

      if L.LemDX > 0 then
        OneWayCheckType := trForceLeft
      else
        OneWayCheckType := trForceRight;

      for i := 1 to 3 do
        if not (HasPixelAt(L.LemX + L.LemDX, L.LemY + i) or
                HasTriggerAt(L.LemX + L.LemDX, L.LemY + i, OneWayCheckType)) then
          Exit;

      Result := true;
    end;
  begin
    // Move upwards if in updraft
    LemYDir := 1;
    if HasTriggerAt(L.LemX, L.LemY, trUpdraft) then LemYDir := -1;

    if    ((FindGroundPixel(L.LemX + L.LemDx, L.LemY) < -4) and DoTurnAround(L, True))
       or (HasConsecutivePixels) then
    begin
      if HasPixelAt(L.LemX, L.LemY) and (LemYDir = 1) then
        fLemNextAction := baWalking
      else if HasPixelAt(L.LemX, L.LemY - 2) and (LemYDir = -1) then
        // Do nothing
      else
        Inc(L.LemY, LemYDir);
    end;
  end;

  function HeadCheck(LemX, Lemy: Integer): Boolean; // returns False if lemming hits his head
  begin
    Result := not (     HasPixelAt(LemX - 1, LemY - 12)
                    and HasPixelAt(LemX, LemY - 12)
                    and HasPixelAt(LemX + 1, LemY - 12));
  end;


const
  GliderFallTable: array[1..17] of Integer =
    (3, 3, 3, 3, -1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
begin
  Result := True;
  MaxFallDist := GliderFallTable[L.LemPhysicsFrame];

  if HasTriggerAt(L.LemX, L.LemY, trUpdraft) then
  begin
    Dec(MaxFallDist);
    // Rise a pixel every second frame
    if (L.LemPhysicsFrame >= 9) and (L.LemPhysicsFrame mod 2 = 1)
       and (not HasPixelAt(L.LemX + L.LemDx, L.LemY + MaxFallDist - 1))
       and HeadCheck(L.LemX, L.LemY - 1) then
      Dec(MaxFallDist);
  end;

  Inc(L.LemX, L.LemDx);

  // Do upwards movement right away
  if MaxFallDist < 0 then Inc(L.LemY, MaxFallDist);

  GroundDist := FindGroundPixel(L.LemX, L.LemY);

  if GroundDist < -4 then // pushed down or turn around
  begin
    if DoTurnAround(L, false) then
    begin
      // move back and turn around
      Dec(L.LemX, L.LemDx);
      TurnAround(L);
      CheckOnePixelShaft(L);
    end
    else
    begin
      // move down
      LemDy := 0;
      repeat
        Inc(LemDy);
      until not HasPixelAt(L.LemX, L.LemY + LemDy);
      Inc(L.LemY, LemDy);
    end
  end

  else if GroundDist < 0 then // Move 1 to 4 pixels up
  begin
    Inc(L.LemY, GroundDist);
    fLemNextAction := baWalking;
  end

  else if MaxFallDist > 0 then // no pixel above current location; not checked if one has moved upwards
  begin // same algorithm as for faller!
    if MaxFallDist > GroundDist then
    begin
      // Lem has found solid terrain
      Assert(GroundDist >= 0, 'glider GroundDist negative');
      Inc(L.LemY, GroundDist);
      fLemNextAction := baWalking;
    end
    else
      Inc(L.LemY, MaxFallDist);
  end

  else if HasTriggerAt(L.LemX, L.LemY, trUpdraft) then // head check for pushing down in updraft
  begin
    // move down at most 2 pixels until the HeadCheck passes
    LemDy := -1;
    while (not HeadCheck(L.LemX, L.LemY)) and (LemDy < 2) do
    begin
      Inc(L.LemY);
      Inc(LemDy);
      // Check whether the glider has reached the ground
      if HasPixelAt(L.LemX, L.LemY) then
      begin
        fLemNextAction := baWalking;
        LemDy := 4;
      end;
    end;
  end;
end;

    */
}