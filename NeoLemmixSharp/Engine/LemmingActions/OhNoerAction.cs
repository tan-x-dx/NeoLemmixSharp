namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class OhNoerAction : LemmingAction
{
    public const int NumberOfOhNoerAnimationFrames = 16;

    public static OhNoerAction Instance { get; } = new();

    private OhNoerAction()
    {
    }

    protected override int ActionId => 20;
    public override string LemmingActionName => "ohnoer";
    public override int NumberOfAnimationFrames => NumberOfOhNoerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
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
}