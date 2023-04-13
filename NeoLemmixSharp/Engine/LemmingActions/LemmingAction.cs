using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.LemmingActions;

public abstract class LemmingAction : IEquatable<LemmingAction>
{
    public static ReadOnlyDictionary<string, LemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, LemmingAction> RegisterAllLemmingActions()
    {
        var ids = new ArrayBasedBitArray(50);

        var result = new Dictionary<string, LemmingAction>();

        RegisterLemmingAction(ids, result, AscenderAction.Instance);
        RegisterLemmingAction(ids, result, BasherAction.Instance);
        RegisterLemmingAction(ids, result, BlockerAction.Instance);
        RegisterLemmingAction(ids, result, BuilderAction.Instance);
        RegisterLemmingAction(ids, result, ClimberAction.Instance);
        RegisterLemmingAction(ids, result, DehoisterAction.Instance);
        RegisterLemmingAction(ids, result, DiggerAction.Instance);
        RegisterLemmingAction(ids, result, DisarmerAction.Instance);
        RegisterLemmingAction(ids, result, DrownerAction.Instance);
        RegisterLemmingAction(ids, result, ExiterAction.Instance);
        RegisterLemmingAction(ids, result, ExploderAction.Instance);
        RegisterLemmingAction(ids, result, FencerAction.Instance);
        RegisterLemmingAction(ids, result, FallerAction.Instance);
        RegisterLemmingAction(ids, result, FloaterAction.Instance);
        RegisterLemmingAction(ids, result, GliderAction.Instance);
        RegisterLemmingAction(ids, result, HoisterAction.Instance);
        RegisterLemmingAction(ids, result, JumperAction.Instance);
        RegisterLemmingAction(ids, result, LasererAction.Instance);
        RegisterLemmingAction(ids, result, MinerAction.Instance);
        RegisterLemmingAction(ids, result, OhNoerAction.Instance);
        RegisterLemmingAction(ids, result, PlatformerAction.Instance);
        RegisterLemmingAction(ids, result, ReacherAction.Instance);
        RegisterLemmingAction(ids, result, ShimmierAction.Instance);
        RegisterLemmingAction(ids, result, ShruggerAction.Instance);
        RegisterLemmingAction(ids, result, SliderAction.Instance);
        RegisterLemmingAction(ids, result, SplatterAction.Instance);
        RegisterLemmingAction(ids, result, StackerAction.Instance);
        RegisterLemmingAction(ids, result, StonerAction.Instance);
        RegisterLemmingAction(ids, result, SwimmerAction.Instance);
        RegisterLemmingAction(ids, result, VaporiserAction.Instance);
        RegisterLemmingAction(ids, result, WalkerAction.Instance);

        return new ReadOnlyDictionary<string, LemmingAction>(result);
    }

    private static void RegisterLemmingAction(IBitArray ids, IDictionary<string, LemmingAction> dict, LemmingAction lemmingAction)
    {
        dict.Add(lemmingAction.LemmingActionName, lemmingAction);

        if (!ids.SetBit(lemmingAction.ActionId))
        {
            throw new Exception($"Duplicated ID: {lemmingAction.LemmingActionName}");
        }
    }

    public static ICollection<LemmingAction> AllLemmingActions => LemmingActions.Values;

    protected static PixelManager Terrain => LevelScreen.CurrentLevel.Terrain;

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    protected abstract int ActionId { get; }
    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }

    public abstract bool UpdateLemming(Lemming lemming);

    public bool Equals(LemmingAction? other) => ActionId == (other?.ActionId ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && Equals(other);
    public sealed override int GetHashCode() => ActionId;

    public static bool operator ==(LemmingAction left, LemmingAction right) => left.ActionId == right.ActionId;
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.ActionId != right.ActionId;

    public virtual void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
        if (turnAround)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
        }

        if (lemming.CurrentAction == this)
            return;

        lemming.CurrentAction = this;
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        var previouslyStartingAction = lemming.IsStartingAction;// because for some actions (eg baHoisting) we need to restore previous value
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;

        //  newAction.OnTransitionToAction(lemming, previouslyStartingAction);
    }
    /*
    protected static void TransitionToNewAction(
        Lemming lemming,
        LemmingAction newAction,
        bool turnAround)
    {
        if (turnAround)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
        }

        if (newAction == WalkerAction.Instance &&////////////////////
            !Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
        {
            newAction = FallerAction.Instance;
        }

        if (lemming.CurrentAction == newAction)
            return;

        // Set initial fall heights according to previous skill
        if (newAction == FallerAction.Instance)//////////////////////////
        {
            if (lemming.CurrentAction != SwimmerAction.Instance)// for Swimming it's set in HandleSwimming as there is no single universal value
            {
                if (lemming.CurrentAction == WalkerAction.Instance ||
                     lemming.CurrentAction == BasherAction.Instance)
                {
                    lemming.DistanceFallen = 3;
                }
                else if (lemming.CurrentAction == MinerAction.Instance ||
                         lemming.CurrentAction == DiggerAction.Instance)
                {
                    lemming.DistanceFallen = 0;
                }
                else if (lemming.CurrentAction == BlockerAction.Instance ||
                         lemming.CurrentAction == JumperAction.Instance ||
                         lemming.CurrentAction == LasererAction.Instance)
                {
                    lemming.DistanceFallen = -1;
                }
                else
                {
                    lemming.DistanceFallen = 1;
                }
            }

            lemming.TrueDistanceFallen = lemming.DistanceFallen;
        }

        if ((lemming.CurrentAction == ClimberAction.Instance &&
             (newAction == ShimmierAction.Instance ||
              newAction == JumperAction.Instance)) ||
            (lemming.CurrentAction == SliderAction.Instance &&
             newAction == JumperAction.Instance))
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
            lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX);

            if (newAction == ShimmierAction.Instance &&
                Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8)).IsSolid)
            {
                lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
            }
        }

        if (newAction == ShimmierAction.Instance)
        {
            if (lemming.CurrentAction == SliderAction.Instance ||
                lemming.CurrentAction == DehoisterAction.Instance)
            {
                lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 2);
                if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8)).IsSolid)
                {
                    lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
                }
            }
            else if (lemming.CurrentAction == JumperAction.Instance)
            {
                for (var i = -1; i < 4; i++)
                {
                    if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 9 + i)).IsSolid &&
                        !Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 8 + i)).IsSolid)
                    {
                        lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, i);
                    }
                }
            }
        }

        if (newAction == DehoisterAction.Instance)
        {
            lemming.DehoistPin = lemming.LevelPosition;
        }
        else if (newAction == SliderAction.Instance)
        {
            lemming.DehoistPin = new Point(-1, -1);
        }

        lemming.CurrentAction = newAction;
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        var previouslyStartingAction = lemming.IsStartingAction;// because for some actions (eg baHoisting) we need to restore previous value
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;

        newAction.OnTransitionToAction(lemming, previouslyStartingAction);
    }*/

    /*
    procedure TLemmingGame.Transition(L: TLemming; NewAction: TBasicLemmingAction; DoTurn: Boolean = False);
{-------------------------------------------------------------------------------
  Handling of a transition and/or turnaround
begin
  if DoTurn then TurnAround(L);

  //Switch from baToWalking to baWalking
  if NewAction = baToWalking then NewAction := baWalking;

  if L.LemHasBlockerField and not (NewAction in [baOhNoing, baStoning]) then
  begin
    L.LemHasBlockerField := False;
    SetBlockerMap;
  end;

  // Transition to faller instead walker, if no pixel below lemming
  if (not HasPixelAt(L.LemX, L.LemY)) and (NewAction = baWalking) then
    NewAction := baFalling;

  // Should not happen, except for assigning walkers to walkers
  if L.LemAction = NewAction then Exit;

  // Set initial fall heights according to previous skill
  if (NewAction = baFalling) then
  begin
    if L.LemAction <> baSwimming then // for Swimming it's set in HandleSwimming as there is no single universal value
    begin
      L.LemFallen := 1;
      if L.LemAction in [baWalking, baBashing] then L.LemFallen := 3
      else if L.LemAction in [baMining, baDigging] then L.LemFallen := 0
      else if L.LemAction in [baBlocking, baJumping, baLasering] then L.LemFallen := -1;
    end;
    L.LemTrueFallen := L.LemFallen;
  end;

  if ((NewAction in [baShimmying, baJumping]) and (L.LemAction = baClimbing)) or
     ((NewAction = baJumping) and (L.LemAction = baSliding)) then
  begin
    // turn around and get out of the wall
    TurnAround(L);
    Inc(L.LemX, L.LemDx);

    if NewAction = baShimmying then
      if HasPixelAt(L.LemX, L.LemY - 8) then
        Inc(L.LemY);
  end;

  if (NewAction = baShimmying) and (L.LemAction = baSliding) then
  begin
    Inc(L.LemY, 2);
    if HasPixelAt(L.LemX, L.LemY - 8) then
      Inc(L.LemY);
  end;

  if (NewAction = baShimmying) and (L.LemAction = baDehoisting) then
  begin
    Inc(L.LemY, 2);
    if HasPixelAt(L.LemX, L.LemY - 9 + 1) then
      Inc(L.LemY);
  end;

  if (NewAction = baShimmying) and (L.LemAction = baJumping) then
  begin
    for i := -1 to 3 do
      if HasPixelAt(L.LemX, L.LemY - 9 - i) and not HasPixelAt(L.LemX, L.LemY - 8 - i) then
      begin
        L.LemY := L.LemY - i;
        Break;
      end;
  end;

  if NewAction = baDehoisting then
    L.LemDehoistPinY := L.LemY;
  if NewAction = baSliding then
    L.LemDehoistPinY := -1;

  // Change Action
  L.LemAction := NewAction;
  L.LemFrame := 0;
  L.LemPhysicsFrame := 0;
  L.LemEndOfAnimation := False;
  L.LemNumberOfBricksLeft := 0;
  OldIsStartingAction := L.LemIsStartingAction; // because for some actions (eg baHoisting) we need to restore previous value
  L.LemIsStartingAction := True;
  L.LemInitialFall := False;

  L.LemMaxFrame := -1;
  L.LemMaxPhysicsFrame := ANIM_FRAMECOUNT[NewAction] - 1;

  // some things to do when entering state
  case L.LemAction of
    baAscending  : L.LemAscended := 0;
    baHoisting   : L.LemIsStartingAction := OldIsStartingAction; // it needs to know what the Climber's value was
    baSplatting  : begin
                     L.LemExplosionTimer := 0;
                     CueSoundEffect(SFX_SPLAT, L.Position)
                   end;
    baBlocking   : begin
                     L.LemHasBlockerField := True;
                     SetBlockerMap;
                   end;
    baExiting    : begin
                     if not IsOutOfTime then
                       L.LemExplosionTimer := 0;
                     CueSoundEffect(SFX_YIPPEE, L.Position);
                   end;
    baVaporizing : L.LemExplosionTimer := 0;
    baBuilding   : begin
                     L.LemNumberOfBricksLeft := 12;
                     L.LemConstructivePositionFreeze := false;
                   end;
    baPlatforming: begin
                     L.LemNumberOfBricksLeft := 12;
                     L.LemConstructivePositionFreeze := false;
                   end;
    baStacking   : L.LemNumberOfBricksLeft := 8;
    baOhnoing,
    baStoning    : begin
                     CueSoundEffect(SFX_OHNO, L.Position);
                     L.LemIsSlider := false;
                     L.LemIsClimber := false;
                     L.LemIsSwimmer := false;
                     L.LemIsFloater := false;
                     L.LemIsGlider := false;
                     L.LemIsDisarmer := false;
                     L.LemHasBeenOhnoer := true;
                   end;
    baExploding  : CueSoundEffect(SFX_EXPLOSION, L.Position);
    baStoneFinish: CueSoundEffect(SFX_EXPLOSION, L.Position);
    baSwimming   : begin // If possible, float up 4 pixels when starting
                     i := 0;
                     while (i < 4) and HasTriggerAt(L.LemX, L.LemY - i - 1, trWater)
                                   and not HasPixelAt(L.LemX, L.LemY - i - 1) do
                       Inc(i);
                     Dec(L.LemY, i);
                   end;
    baFixing     : L.LemDisarmingFrames := 42;
    baJumping    : L.LemJumpProgress := 0;
    baLasering   : L.LemLaserRemainTime := 10;
  end;
end;

    */

    protected static void LayBrick(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var brickDelta = lemming.CurrentAction == BuilderAction.Instance
            ? 1
            : 0;

        var brickPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, brickDelta);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);
    }

    protected static int FindGroundPixel(
        IOrientation orientation,
        Point levelPosition)
    {
        // Find the new ground pixel
        // If Result = 4, then at least 4 pixels are air below (X, Y)
        // If Result = -7, then at least 7 pixels are terrain above (X, Y)
        var result = 0;
        if (Terrain.GetPixelData(levelPosition).IsSolid)
        {
            while (Terrain.GetPixelData(orientation.MoveUp(levelPosition, 1 - result)).IsSolid &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!Terrain.GetPixelData(orientation.MoveDown(levelPosition, result)).IsSolid &&
               result < 4)
        {
            result++;
        }

        return result;
    }

    protected static bool LemmingCanDehoist(Lemming lemming, bool b)
    {
        return false;
    }
}