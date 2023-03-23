namespace NeoLemmixSharp.Engine.LemmingActions;

public static class CommonMethods
{
    public static void LayBrick(Lemming lemming)
    {
        var brickPosition = lemming.LevelPosition;
        if (lemming.CurrentAction == BuilderAction.Instance)
        {
            brickPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
        }

        var deltaX = lemming.FacingDirection.DeltaX(1);
        for (var i = 0; i < 5; i++)
        {
            LevelScreen.CurrentLevel!.Terrain.SetSolidPixel(brickPosition, uint.MaxValue);
            brickPosition = lemming.Orientation.MoveRight(brickPosition, deltaX);
        }
    }

    public static void TransitionToNewAction(
        Lemming lemming,
        ILemmingAction newAction,
        bool turnAround)
    {
        if (turnAround)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
        }


        var pixel = LevelScreen.CurrentLevel!.Terrain.GetPixelData(ref lemming.LevelPosition);
        if (!pixel.IsSolid)
        {
            newAction = FallerAction.Instance;
        }

        if (lemming.CurrentAction == newAction)
            return;

       /* if (newAction == FallerAction.Instance)
        {
            if (lemming.CurrentAction != SwimmerAction.Instance)
            {
                if (lemming.CurrentAction == WalkerAction.Instance ||
                     lemming.CurrentAction == BasherAction.Instance)
                {
                    lemming.LemFallen = 3;
                }
                else if (lemming.CurrentAction == MinerAction.Instance ||
                         lemming.CurrentAction == DiggerAction.Instance)
                {
                    lemming.LemFallen = 0;
                }
                else if (lemming.CurrentAction == BlockerAction.Instance ||
                         lemming.CurrentAction == JumperAction.Instance ||
                         lemming.CurrentAction == LasererAction.Instance)
                {
                    lemming.LemFallen = -1;
                }
                else
                {
                    lemming.LemFallen = 1;
                }
            }

            lemming.LemTrueFallen = lemming.LemFallen;
        }*/
    }

    /*

    -------------------------------------------------------------------------------
  Handling of a transition and/or turnaround
-------------------------------------------------------------------------------}
var
  i: Integer;
  OldIsStartingAction: Boolean;
const
  // Number of physics frames for the various lemming actions.
  ANIM_FRAMECOUNT: array[TBasicLemmingAction] of Integer =
    (
     0, //baNone,
     4, //baWalking,
     1, //baAscending,
    16, //baDigging,
     8, //baClimbing,
    16, //baDrowning,
     8, //baHoisting,
    16, //baBuilding,
    16, //baBashing,
    24, //baMining,
     4, //baFalling,
    17, //baFloating,
    16, //baSplatting,
     8, //baExiting,
    14, //baVaporizing,
    16, //baBlocking,
     8, //baShrugging,
    16, //baOhnoing,
     1, //baExploding,
     0, //baToWalking,
    16, //baPlatforming,
     8, //baStacking,
    16, //baStoning,
     1, //baStoneFinish,
     8, //baSwimming,
    17, //baGliding,
    16, //baFixing,
     0, //baCloning,
    16, //baFencing,
     8, //baReaching,
    20, //baShimmying
    13, //baJumping
     7, //baDehoisting
     1, //baSliding
    12  //baLasering - it's, ironically, this high for rendering purposes
    );
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
}