using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds
{
    private static TerrainManager TerrainManager { get; set; } = null!;
    private static LemmingManager LemmingManager { get; set; } = null!;
    private static GadgetManager GadgetManager { get; set; } = null!;

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        TerrainManager = terrainManager;
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        LemmingManager = lemmingManager;
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
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
    public bool JumpToHoistAdvance;

    public bool Debug;

    public int AnimationFrame;
    public int PhysicsFrame;
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

    public LevelPosition HeadPosition => Orientation.MoveUp(LevelPosition, 6);
    public LevelPosition FootPosition => Orientation.MoveUp(LevelPosition, 1);

    public FacingDirection FacingDirection { get; private set; }
    public Orientation Orientation { get; private set; }

    public LemmingAction PreviousAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; } = NoneAction.Instance;

    public LemmingRenderer Renderer { get; private set; } = null!;
    public LemmingState State { get; }

    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
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
        State = new LemmingState(this, Team.AllItems[0]);
    }

    public void SetRenderer(LemmingRenderer lemmingRenderer)
    {
        Renderer = lemmingRenderer;
    }

    public void Initialise()
    {
        var levelPositionPair = CurrentAction.GetLemmingBounds(this);
        TopLeftPixel = levelPositionPair.GetTopLeftPosition();
        BottomRightPixel = levelPositionPair.GetBottomRightPosition();

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;
    }

    public void Tick()
    {
        if (Debug)
        {
            ;
        }

        PreviousAction = CurrentAction;
        // No transition to do at the end of lemming movement
        NextAction = NoneAction.Instance;

        _ = HandleLemmingAction() && CheckLevelBoundaries() && CheckTriggerArea(false);
    }

    private bool HandleLemmingAction()
    {
        PhysicsFrame++;

        if (PhysicsFrame >= CurrentAction.NumberOfAnimationFrames)
        {
            if (CurrentAction == FloaterAction.Instance ||
                CurrentAction == GliderAction.Instance)
            {
                PhysicsFrame = 9;
            }
            else
            {
                PhysicsFrame = 0;
            }

            if (CurrentAction.IsOneTimeAction)
            {
                EndOfAnimation = true;
            }
        }

        AnimationFrame = PhysicsFrame;
        // AnimationFrame is usually identical to PhysicsFrame
        // Exceptions occur in JumperAction, for example

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;

        var result = CurrentAction.UpdateLemming(this);
        var levelPositionPair = CurrentAction.GetLemmingBounds(this);

        TopLeftPixel = levelPositionPair.GetTopLeftPosition();
        BottomRightPixel = levelPositionPair.GetBottomRightPosition();

        return result;
    }

    private bool CheckLevelBoundaries()
    {
        var footPixel = TerrainManager.PixelTypeAtPosition(FootPosition);
        var headPixel = TerrainManager.PixelTypeAtPosition(HeadPosition);

        if (!footPixel.IsVoid() || !headPixel.IsVoid())
            return true;

        LemmingManager.RemoveLemming(this);
        return false;
    }

    private bool CheckTriggerArea(bool isPostTeleportCheck)
    {
        var needShiftPosition = false;

        var currentAnchorPixel = LevelPosition;
        var currentFootPixel = Orientation.MoveUp(currentAnchorPixel, 1);

        LevelPosition previousAnchorPixel, previousFootPixel;

        if (isPostTeleportCheck)
        {
            PreviousLevelPosition = currentAnchorPixel;
            previousAnchorPixel = currentAnchorPixel;
            previousFootPixel = currentFootPixel;
        }
        else
        {
            previousAnchorPixel = PreviousLevelPosition;
            previousFootPixel = Orientation.MoveUp(previousAnchorPixel, 1);
        }

        var checkRegion = new LevelPositionPair(currentAnchorPixel, currentFootPixel, previousAnchorPixel, previousFootPixel);

        var topLeftPixel = checkRegion.GetTopLeftPosition();
        var bottomRightPixel = checkRegion.GetBottomRightPosition();

        CheckGadgets(topLeftPixel, bottomRightPixel);

        NextAction.TransitionLemmingToAction(this, false);

        return true;
    }

    private void CheckGadgets(LevelPosition topLeftPixel, LevelPosition bottomRightPixel)
    {
        var gadgetEnumerator = GadgetManager.GetAllItemsNearRegion(topLeftPixel, bottomRightPixel);

        if (gadgetEnumerator.IsEmpty)
            return;

        Span<LevelPosition> checkPositions = stackalloc LevelPosition[LemmingMovementHelper.MaxIntermediateCheckPositions];
        var movementHelper = new LemmingMovementHelper(this, checkPositions);
        movementHelper.EvaluateCheckPositions();
        var length = movementHelper.Length;

        for (var i = 0; i < length; i++)
        {
            var anchorPosition = checkPositions[i];
            var footPosition = Orientation.MoveUp(anchorPosition, 1);

            while (gadgetEnumerator.MoveNext())
            {
                var gadget = gadgetEnumerator.Current;

                if (gadget.MatchesLemmingAtPosition(this, anchorPosition) ||
                    gadget.MatchesLemmingAtPosition(this, footPosition))
                {
                    var beforeAction = CurrentAction;
                    HandleGadgetInteraction(gadget, anchorPosition);
                    var afterAction = CurrentAction;

                    if (beforeAction != afterAction)
                        return;
                }
            }

            gadgetEnumerator.Reset();
        }
    }

    private void HandleGadgetInteraction(HitBoxGadget gadget, LevelPosition checkPosition)
    {
        // Transition if we are at the end position and need to do one
        // Except if we try to splat and there is water at the lemming position - then let this take precedence.
        if (NextAction != NoneAction.Instance &&
            checkPosition == LevelPosition &&
            (NextAction != SplatterAction.Instance || gadget.Type != WaterGadgetType.Instance))
        {
            NextAction.TransitionLemmingToAction(this, false);
            if (JumpToHoistAdvance)
            {
                AnimationFrame += 2;
                PhysicsFrame += 2;
            }

            NextAction = NoneAction.Instance;
            JumpToHoistAdvance = false;
        }

        gadget.OnLemmingMatch(this, checkPosition);
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
        Renderer.UpdateLemmingState(true);
    }

    public void SetOrientation(Orientation newOrientation)
    {
        Orientation = newOrientation;
        Renderer.UpdateLemmingState(true);
    }

    public void SetCurrentAction(LemmingAction lemmingAction)
    {
        CurrentAction = lemmingAction;
        Renderer.UpdateLemmingState(true);
    }

    public void SetNextAction(LemmingAction nextAction)
    {
        NextAction = nextAction;
    }

    public void OnInitialization()
    {
        Renderer.UpdateLemmingState(true);
    }

    public void OnRemoval()
    {
        CurrentAction = NoneAction.Instance;
        Renderer.UpdateLemmingState(false);
    }

    public bool Equals(Lemming? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Lemming other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Lemming left, Lemming right) => left.Id == right.Id;
    public static bool operator !=(Lemming left, Lemming right) => left.Id != right.Id;
}