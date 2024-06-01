using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class GliderAction : LemmingAction
{
    public static readonly GliderAction Instance = new();

    private readonly int[] _gliderFallTable = [3, 3, 3, 3, -1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];

    private GliderAction()
        : base(
            LevelConstants.GliderActionId,
            LevelConstants.GliderActionName,
            LevelConstants.GliderAnimationFrames,
            LevelConstants.MaxGliderPhysicsFrames,
            LevelConstants.PermanentSkillPriority,
            false,
            true)
    {
    }

    /*
function TLemmingGame.HandleGliding(L: TLemming): Boolean;
var
  MaxFallDist, GroundDist: Integer;
  LemDy: Integer;

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
    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var dx = lemming.FacingDirection.DeltaX;

        var maxFallDistance = _gliderFallTable[lemming.PhysicsFrame];

        var updraftFallDelta = FallerAction.GetUpdraftFallDelta(lemming);

        var gadgetTestRegion = new LevelPositionPair(
            orientation.Move(lemmingPosition, -1, -12),
            orientation.Move(lemmingPosition, 1, LevelConstants.MaxStepUp + 1));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        if (updraftFallDelta.Y < 0)
        {
            maxFallDistance--;

            // Rise a pixel every second frame
            if (lemming.PhysicsFrame >= 9 &&
                (lemming.PhysicsFrame & 1) != 0 &&
                !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 1 - maxFallDistance)) &&
                HeadCheck(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 1)))
            {
                maxFallDistance--;
            }
        }

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

        // Do upwards movement right away
        if (maxFallDistance < 0)
        {
            // Moving down, but by a negative amount so going up
            lemmingPosition = orientation.MoveDown(lemmingPosition, maxFallDistance);
        }

        var groundDistance = FindGroundPixel(lemming, lemmingPosition);

        int dy;

        if (groundDistance > 4) // Pushed down or turn around
        {
            if (DoTurnAround(in gadgetsNearRegion, lemming, false)) // Move back and turn around
            {
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
                CheckOnePixelShaft(in gadgetsNearRegion, lemming);
                return true;
            }

            dy = 0;
            LevelPosition checkPosition;
            do
            {
                dy++;
                checkPosition = orientation.MoveDown(lemmingPosition, dy);
            } while (!PositionIsSolidToLemming(gadgetsNearRegion, lemming, checkPosition));

            lemmingPosition = checkPosition;

            return true;
        }

        if (groundDistance > 0) // Move 1 to 4 pixels up
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, groundDistance);
            lemming.SetNextAction(WalkerAction.Instance);

            return true;
        }

        if (maxFallDistance > 0) // No pixel above current location; not checked if one has moved upwards
        {
            // Same algorithm as for faller!

            if (maxFallDistance > -groundDistance)
            {
                // Lem has found solid terrain
                lemmingPosition = orientation.MoveUp(lemmingPosition, groundDistance);
                lemming.SetNextAction(WalkerAction.Instance);

                return true;
            }

            lemmingPosition = orientation.MoveDown(lemmingPosition, maxFallDistance);

            return true;
        }

        updraftFallDelta = FallerAction.GetUpdraftFallDelta(lemming);
        if (updraftFallDelta.Y >= 0)
            return true; // Head check for pushing down in updraft

        // Move down at most 2 pixels until the HeadCheck passes

        dy = -1;
        while (!HeadCheck(in gadgetsNearRegion, lemming, lemmingPosition) && dy < 2)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            dy++;

            // Check whether the glider has reached the ground
            if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
            {
                lemming.SetNextAction(WalkerAction.Instance);
                return true;
            }
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 12;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 1;

    private static bool DoTurnAround(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming,
        bool moveForwardFirst)
    {
        var orientation = lemming.Orientation;
        var checkPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (moveForwardFirst)
        {
            checkPosition = orientation.MoveRight(checkPosition, dx);
        }

        var dy = 0;
        do
        {
            // Bug-fix for http://www.lemmingsforums.net/index.php?topic=2693
            if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveDown(checkPosition, dy)) &&
                PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(checkPosition, -dx, dy)))
                // Abort computation and let lemming turn around
                return true;

            dy++;

        } while (dy <= 3 && PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveDown(checkPosition, dy)));

        return dy > 3;
    }

    // Special behavior in 1-pixel wide shafts: Move one pixel down even when turning
    private static void CheckOnePixelShaft(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var dx = lemming.FacingDirection.DeltaX;
        var groundPixelDelta = FindGroundPixel(lemming, orientation.MoveRight(lemmingPosition, dx));

        if ((groundPixelDelta <= 4 ||
             !DoTurnAround(in gadgetsNearRegion, lemming, true)) &&
            !HasConsecutivePixels(in gadgetsNearRegion))
            return;

        var updraftFallDelta = FallerAction.GetUpdraftFallDelta(lemming);
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition) &&
            updraftFallDelta.Y >= 0)
        {
            lemming.SetNextAction(WalkerAction.Instance);

            return;
        }

        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 2)) &&
            updraftFallDelta.Y < 0)
            return;

        var updraftDy = updraftFallDelta.Y < 0
            ? 1
            : -1;
        lemmingPosition = orientation.MoveUp(lemmingPosition, updraftDy);

        return;

        bool HasConsecutivePixels(in GadgetSet gadgetsNearRegion1)
        {
            // Check at LemY +1, +2, +3 for (a) solid terrain, or (b) a one-way field that will turn the lemming around
            var checkPosition = orientation.MoveRight(lemming.LevelPosition, dx);

            for (var i = 1; i < 4; i++)
            {
                if (!PositionIsSolidToLemming(gadgetsNearRegion1, lemming, checkPosition))
                    return false;
            }

            return true;
        }
    }

    private static bool HeadCheck(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming,
        LevelPosition checkPosition)
    {
        var orientation = lemming.Orientation;

        return !(PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(checkPosition, -1, 12)) ||
                 PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(checkPosition, 0, 12)) ||
                 PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(checkPosition, 1, 12)));
    }
}