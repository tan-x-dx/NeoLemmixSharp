using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class OhNoerAction : LemmingAction
{
    public static readonly OhNoerAction Instance = new();

    private OhNoerAction()
    {
    }

    public override int Id => LevelConstants.OhNoerActionId;
    public override string LemmingActionName => "ohnoer";
    public override int NumberOfAnimationFrames => LevelConstants.OhNoerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var result = true;

        if (lemming.EndOfAnimation)
        {
            //   if(lemming.CurrentAction == )

        }
        else if (!LevelConstants.TerrainManager.PixelIsSolidToLemming(lemming, lemming.LevelPosition))
        {
            LevelConstants.LemmingManager.DeregisterBlocker(lemming);
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

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => animationFrame < 7 ? 10 : 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

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
}