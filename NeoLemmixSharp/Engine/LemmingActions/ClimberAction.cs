namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ClimberAction : LemmingAction
{
    public const int NumberOfClimberAnimationFrames = 8;

    public static ClimberAction Instance { get; } = new();

    private ClimberAction()
    {
    }

    public override string LemmingActionName => "climber";
    public override int NumberOfAnimationFrames => NumberOfClimberAnimationFrames;
    public override bool IsOneTimeAction => false;

    // Be very careful when changing the terrain/hoister checks for climbers!
    // See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        if (lemming.AnimationFrame <= 3)
        {
            var foundClip =
                Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 6 + lemming.AnimationFrame))
                    .IsSolid ||
                Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 5 + lemming.AnimationFrame))
                    .IsSolid &&
                !lemming.IsStartingAction;

            if (lemming.AnimationFrame == 0) // first triggered after 8 frames!
            {
                foundClip &= Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 7)).IsSolid;
            }

            if (foundClip)
            {
                // Don't fall below original position on hitting terrain in first cycle
                if (!lemming.IsStartingAction)
                {
                    lemming.LevelPosition =
                        lemming.Orientation.MoveDown(lemming.LevelPosition, lemming.AnimationFrame - 3);
                }

                if (lemming.IsSlider)
                {
                    lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
                    CommonMethods.TransitionToNewAction(lemming, SliderAction.Instance, false);
                }
                else
                {
                    lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
                    CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, true);
                    lemming.DistanceFallen++; // Least-impact way to fix a fall distance inconsistency. See https://www.lemmingsforums.net/index.php?topic=5794.0
                }
            }
            else if (!Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 7 + lemming.AnimationFrame)).IsSolid)
            {
                // if-case prevents too deep bombing, see http://www.lemmingsforums.net/index.php?topic=2620.0
                if (!(lemming.IsStartingAction && lemming.AnimationFrame == 1))
                {
                    lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 2 - lemming.AnimationFrame);
                    lemming.IsStartingAction = false;
                }

                CommonMethods.TransitionToNewAction(lemming, HoisterAction.Instance, false);
            }
        }
        else
        {
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
            lemming.IsStartingAction = false;

            var foundClip = Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 7)).IsSolid;

            if (lemming.AnimationFrame == 7)
            {
                foundClip = foundClip && Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 7)).IsSolid;
            }

            if (foundClip)
            {
                lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);

                if (lemming.IsSlider)
                {
                    CommonMethods.TransitionToNewAction(lemming, SliderAction.Instance, false);
                }
                else
                {
                    lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
                    CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, true);
                }
            }
        }

        return true;
    }

    /*
    
function TLemmingGame.HandleClimbing(L: TLemming): Boolean;
// Be very careful when changing the terrain/hoister checks for climbers!
// See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
var
  FoundClip: Boolean;
begin
  Result := True;

  if L.LemPhysicsFrame <= 3 then
  begin
    FoundClip := (HasPixelAt(L.LemX - L.LemDx, L.LemY - 6 - L.LemPhysicsFrame))
              or (HasPixelAt(L.LemX - L.LemDx, L.LemY - 5 - L.LemPhysicsFrame) and (not L.LemIsStartingAction));

    if L.LemPhysicsFrame = 0 then // first triggered after 8 frames!
      FoundClip := FoundClip and HasPixelAt(L.LemX - L.LemDx, L.LemY - 7);

    if FoundClip then
    begin
      // Don't fall below original position on hitting terrain in first cycle
      if not L.LemIsStartingAction then L.LemY := L.LemY - L.LemPhysicsFrame + 3;

      if L.LemIsSlider then
      begin
        Dec(L.LemY);
        Transition(L, baSliding);
      end else begin
        Dec(L.LemX, L.LemDx);
        Transition(L, baFalling, True); // turn around as well
        Inc(L.LemFallen); // Least-impact way to fix a fall distance inconsistency. See https://www.lemmingsforums.net/index.php?topic=5794.0
      end;
    end
    else if not HasPixelAt(L.LemX, L.LemY - 7 - L.LemPhysicsFrame) then
    begin
      // if-case prevents too deep bombing, see http://www.lemmingsforums.net/index.php?topic=2620.0
      if not (L.LemIsStartingAction and (L.LemPhysicsFrame = 1)) then
      begin
        L.LemY := L.LemY - L.LemPhysicsFrame + 2;
        L.LemIsStartingAction := False;
      end;
      Transition(L, baHoisting);
    end;
  end

  else
  begin
    Dec(L.LemY);
    L.LemIsStartingAction := False;

    FoundClip := HasPixelAt(L.LemX - L.LemDx, L.LemY - 7);

    if L.LemPhysicsFrame = 7 then
      FoundClip := FoundClip and HasPixelAt(L.LemX, L.LemY - 7);

    if FoundClip then
    begin
      Inc(L.LemY);

      if L.LemIsSlider then
        Transition(L, baSliding)
      else begin
        Dec(L.LemX, L.LemDx);
        Transition(L, baFalling, True); // turn around as well
      end;
    end;
  end;
end;

    */

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}