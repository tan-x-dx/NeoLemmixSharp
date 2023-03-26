using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class OhNoerAction : ILemmingAction
{
    public const int NumberOfOhNoerAnimationFrames = 16;

    public static OhNoerAction Instance { get; } = new();

    private OhNoerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "ohnoer";
    public int NumberOfAnimationFrames => NumberOfOhNoerAnimationFrames;
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is OhNoerAction;
    public override bool Equals(object? obj) => obj is OhNoerAction;
    public override int GetHashCode() => nameof(OhNoerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        var result = true;

        if (lemming.EndOfAnimation)
        {
         //   if(lemming.CurrentAction == )

        }
        else if (!Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
        {
            /*
            L.LemHasBlockerField := False; // remove blocker field
            SetBlockerMap;
            // let lemming fall
            if HasTriggerAt(L.LemX, L.LemY, trUpdraft) then
              Inc(L.LemY, MinIntValue([FindGroundPixel(L.LemX, L.LemY), 2]))
            else
              Inc(L.LemY, MinIntValue([FindGroundPixel(L.LemX, L.LemY), 3]));
             */
        }

        return result;
    }

    /*
function TLemmingGame.HandleOhNoing(L: TLemming): Boolean;
begin
  Result := True;
  if L.LemEndOfAnimation then
  begin
    if L.LemAction = baOhNoing then
      Transition(L, baExploding)
    else // if L.LemAction = baStoning then
      Transition(L, baStoneFinish);
    L.LemHasBlockerField := False; // remove blocker field
    SetBlockerMap;
    Result := False;
  end
  else if not HasPixelAt(L.LemX, L.LemY) then
  begin
    L.LemHasBlockerField := False; // remove blocker field
    SetBlockerMap;
    // let lemming fall
    if HasTriggerAt(L.LemX, L.LemY, trUpdraft) then
      Inc(L.LemY, MinIntValue([FindGroundPixel(L.LemX, L.LemY), 2]))
    else
      Inc(L.LemY, MinIntValue([FindGroundPixel(L.LemX, L.LemY), 3]));
  end;
end;
    */

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}