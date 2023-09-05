using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds
{
    private static TerrainManager TerrainManager { get; set; }
    private static GadgetManager GadgetManager { get; set; }

    public static void SetHelpers(TerrainManager terrainManager, GadgetManager gadgetManager)
    {
        TerrainManager = terrainManager;
        GadgetManager = gadgetManager;
    }

    public int Id { get; }

    public bool ConstructivePositionFreeze;
    public bool IsStartingAction;
    public bool PlacedBrick;
    public bool StackLow;
    public bool InitialFall;
    public bool EndOfAnimation;
    public bool LaserHit;

    public bool Debug;

    public int AnimationFrame;//{ get; set; }
    public int AscenderProgress;
    public int NumberOfBricksLeft;
    public int DisarmingFrames;
    public int DistanceFallen;
    public int TrueDistanceFallen;
    public int LaserRemainTime;

    public int FastForwardTime = 0;

    public bool IsFastForward => FastForwardTime > 0;

    public LevelPosition DehoistPin;
    public LevelPosition LaserHitLevelPosition;
    public LevelPosition LevelPosition;
    public LevelPosition PreviousLevelPosition;

    public FacingDirection FacingDirection { get; private set; }
    public Orientation Orientation { get; private set; }

    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; } = NoneAction.Instance;

    public LemmingRenderer Renderer { get; }
    public LemmingState State { get; }

    public bool ShouldTick => true;

    public LevelPosition TopLeftPixel => LevelPosition + new LevelPosition(-3, -3);
    public LevelPosition BottomRightPixel => LevelPosition + new LevelPosition(2, 2);
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public Lemming(
        int id,
        Orientation? orientation = null,
        FacingDirection? facingDirection = null,
        LemmingAction? currentAction = null)
    {
        Id = id;
        Orientation = orientation ?? DownOrientation.Instance;
        FacingDirection = facingDirection ?? RightFacingDirection.Instance;
        CurrentAction = currentAction ?? WalkerAction.Instance;
        State = new LemmingState(Team.AllItems[0]);

        Renderer = new LemmingRenderer(this);
    }

    public void Tick()
    {
        if (Debug)
        {
            ;
        }

        var continueWithLemming = true;
        var oldLevelPosition = LevelPosition;
        var oldFacingDirection = FacingDirection;
        var oldAction = CurrentAction;
        NextAction = NoneAction.Instance;

        _ = HandleLemmingAction() && CheckLevelBoundaries() && CheckTriggerArea(false);

        /* if (!continueWithLemming)
             return;
         continueWithLemming = HandleLemmingAction();
         if (!continueWithLemming)
             return;
         continueWithLemming = CheckLevelBoundaries();
         if (!continueWithLemming)
             return;
         CheckTriggerArea(false);*/
    }

    private bool HandleLemmingAction()
    {
        AnimationFrame++;
        if (AnimationFrame == CurrentAction.NumberOfAnimationFrames)
        {
            if (CurrentAction == FloaterAction.Instance ||
                CurrentAction == GliderAction.Instance)
            {
                AnimationFrame = 9;
            }
            else
            {
                AnimationFrame = 0;
            }

            if (CurrentAction.IsOneTimeAction)
            {
                EndOfAnimation = true;
            }
        }

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;
        var result = CurrentAction.UpdateLemming(this);

        return result;
    }

    private bool CheckLevelBoundaries()
    {
        var footPixel = TerrainManager.PixelTypeAtPosition(Orientation.MoveUp(LevelPosition, 1));
        var headPixel = TerrainManager.PixelTypeAtPosition(Orientation.MoveUp(LevelPosition, 6));

        if (footPixel.IsVoid() && headPixel.IsVoid())
        {
            //

            return false;
        }

        return true;
    }

    private bool CheckTriggerArea(bool isPostTeleportCheck)
    {
        if (isPostTeleportCheck)
        {

        }

        var gadgetEnumerator = GadgetManager.GetAllGadgetsForPosition(LevelPosition);
        while (gadgetEnumerator.MoveNext())
        {
            var gadget = gadgetEnumerator.Current;

            if (gadget.MatchesLemming(this))
            {
                gadget.OnLemmingMatch(this);
            }
        }

        NextAction.TransitionLemmingToAction(this, false);

        return true;
    }

    /*
procedure TLemmingGame.CheckTriggerArea(L: TLemming; IsPostTeleportCheck: Boolean = false);
// For intermediate pixels, we call the trigger function according to trigger area
var
  CheckPos: TArrayArrayInt; // Combined list for both X- and Y-coordinates
  i: Integer;
  AbortChecks: Boolean;

  NeedShiftPosition: Boolean;
  SavePos: TPoint;
begin
  // If this is a post-teleport check, (a) reset previous position and (b) remember new position
  if IsPostTeleportCheck then
  begin
    L.LemXOld := L.LemX;
    L.LemYOld := L.LemY;
    SavePos := Point(L.LemX, L.LemY);
  end;

  // Get positions to check for trigger areas
  CheckPos := GetGadgetCheckPositions(L);

  // Now move through the values in CheckPosX/Y and check for trigger areas
  i := -1;
  AbortChecks := False;
  NeedShiftPosition := False;
  repeat
    Inc(i);

    // Make sure, that we do not move outside the range of CheckPos.
    Assert(i <= Length(CheckPos[0]), 'CheckTriggerArea: CheckPos has not enough entries');
    Assert(i <= Length(CheckPos[1]), 'CheckTriggerArea: CheckPos has not enough entries');

    // Transition if we are at the end position and need to do one
    // Except if we try to splat and there is water at the lemming position - then let this take precedence.
    if (fLemNextAction <> baNone) and ([CheckPos[0, i], CheckPos[1, i]] = [L.LemX, L.LemY])
      and ((fLemNextAction <> baSplatting) or not HasTriggerAt(L.LemX, L.LemY, trWater)) then
    begin
      Transition(L, fLemNextAction);
      if fLemJumpToHoistAdvance then
      begin
        Inc(L.LemFrame, 2);
        Inc(L.LemPhysicsFrame, 2);
      end;

      fLemNextAction := baNone;
      fLemJumpToHoistAdvance := false;
    end;

    // Pickup Skills
    if HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trPickup) then
      HandlePickup(L, CheckPos[0, i], CheckPos[1, i]);

    // Buttons
    if HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trButton) then
      HandleButton(L, CheckPos[0, i], CheckPos[1, i]);

    // Fire
    if HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trFire) then
      AbortChecks := HandleFire(L);

    // Water - Check only for drowning here!
    if (not AbortChecks) and HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trWater) then
      AbortChecks := HandleWaterDrown(L);

    // Triggered traps and one-shot traps
    if (not AbortChecks) and HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trTrap) then
    begin
      AbortChecks := HandleTrap(L, CheckPos[0, i], CheckPos[1, i]);
      // Disarmers move always to final X-position, see http://www.lemmingsforums.net/index.php?topic=3004.0
      if (L.LemAction = baFixing) then CheckPos[0, i] := L.LemX;
    end;

    // Teleporter
    if (not AbortChecks) and HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trTeleport) and not IsPostTeleportCheck then
      AbortChecks := HandleTeleport(L, CheckPos[0, i], CheckPos[1, i]);

    // Exits
    if (not AbortChecks) and HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trExit) then
      AbortChecks := HandleExit(L, CheckPos[0, i], CheckPos[1, i]);

    // Flipper (except for blockers / jumpers)
    if (not AbortChecks) and HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trFlipper)
                         and not (L.LemAction = baBlocking)
                         and not ((L.LemActionOld = baJumping) or (L.LemAction = baJumping)) then
    begin
      NeedShiftPosition := (L.LemAction in [baClimbing, baSliding, baDehoisting]);
      AbortChecks := HandleFlipper(L, CheckPos[0, i], CheckPos[1, i]);
      NeedShiftPosition := NeedShiftPosition and AbortChecks;
    end;

    // Triggered animations and one-shot animations
    if (not AbortChecks) and HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trAnim) then
      HandleAnimation(L, CheckPos[0, i], CheckPos[1, i]); // HandleAnimation will never activate AbortChecks

    // If the lem was required stop, move him there!
    if AbortChecks then
    begin
      L.LemX := CheckPos[0, i];
      L.LemY := CheckPos[1, i];
    end;

    // Set L.LemInFlipper correctly
    if not HasTriggerAt(CheckPos[0, i], CheckPos[1, i], trFlipper)
       and not ((L.LemActionOld = baJumping) or (L.LemAction = baJumping)) then
      L.LemInFlipper := DOM_NOOBJECT;
  until (CheckPos[0, i] = L.LemX) and (CheckPos[1, i] = L.LemY) (*or AbortChecks*);

  if NeedShiftPosition then
    Inc(L.LemX, L.LemDX);

  // Check for water to transition to swimmer only at final position
  if HasTriggerAt(L.LemX, L.LemY, trWater) then
    HandleWaterSwim(L);

  // Check for blocker fields and force-fields
  // but not for miners removing terrain, see http://www.lemmingsforums.net/index.php?topic=2710.0
  // also not for Jumpers, as this is handled during movement
  if ((L.LemAction <> baMining) or not (L.LemPhysicsFrame in [1, 2])) and
     (L.LemAction <> baJumping) then
  begin
    if HasTriggerAt(L.LemX, L.LemY, trForceLeft, L) then
      HandleForceField(L, -1)
    else if HasTriggerAt(L.LemX, L.LemY, trForceRight, L) then
      HandleForceField(L, 1);
  end;

  // And if this was a post-teleporter check, reset any position changes that may have occurred.
  if IsPostTeleportCheck then
  begin
    L.LemX := SavePos.X;
    L.LemY := SavePos.Y;
  end;
end;

     */
    public void SetFacingDirection(FacingDirection newFacingDirection)
    {
        FacingDirection = newFacingDirection;
        Renderer.UpdateLemmingState();
    }

    public void SetOrientation(Orientation newOrientation)
    {
        Orientation = newOrientation;
        Renderer.UpdateLemmingState();
    }

    public void SetCurrentAction(LemmingAction lemmingAction)
    {
        CurrentAction = lemmingAction;
        Renderer.UpdateLemmingState();
    }

    public void SetNextAction(LemmingAction nextAction)
    {
        NextAction = nextAction;
    }

    public bool Equals(Lemming? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Lemming other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Lemming left, Lemming right) => left.Id == right.Id;
    public static bool operator !=(Lemming left, Lemming right) => left.Id != right.Id;
}