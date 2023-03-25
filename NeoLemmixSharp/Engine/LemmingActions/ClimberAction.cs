using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ClimberAction : ILemmingAction
{
    public const int NumberOfClimberAnimationFrames = 8;

    public static ClimberAction Instance { get; } = new();

    private ClimberAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "climber";
    public int NumberOfAnimationFrames => NumberOfClimberAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is ClimberAction;
    public override bool Equals(object? obj) => obj is ClimberAction;
    public override int GetHashCode() => nameof(ClimberAction).GetHashCode();

    // Be very careful when changing the terrain/hoister checks for climbers!
    // See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame <= 3)
        {
            var dx = lemming.FacingDirection.DeltaX;
            var foundClip = Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 6 + lemming.AnimationFrame)).IsSolid ||
                            Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 5 + lemming.AnimationFrame)).IsSolid &&
                            !lemming.IsStartingAction;

            if (lemming.AnimationFrame == 0)// first triggered after 8 frames!
            {
                foundClip &= Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, -dx, 7 + lemming.AnimationFrame)).IsSolid;
            }

            if (foundClip)
            {
                // Don't fall below original position on hitting terrain in first cycle
                if (!lemming.IsStartingAction)
                {
                    lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, lemming.AnimationFrame - 3);
                }

                var isSlider = false;//TODO
                if (isSlider)
                {
                    lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
                    CommonMethods.TransitionToNewAction(lemming, SliderAction.Instance, false);
                }
                else
                {
                    lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
                    CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, true);
                }
            }
        }
        else
        {

        }
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

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}