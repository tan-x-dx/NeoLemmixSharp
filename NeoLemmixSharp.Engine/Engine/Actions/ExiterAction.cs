namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ExiterAction : LemmingAction
{
    public static ExiterAction Instance { get; } = new();

    private ExiterAction()
    {
    }

    public override int Id => GameConstants.ExiterActionId;
    public override string LemmingActionName => "exiter";
    public override int NumberOfAnimationFrames => GameConstants.ExiterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => 2;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    /*
function TLemmingGame.HandleExiting(L: TLemming): Boolean;
begin
  Result := False;

  if IsOutOfTime then
  begin
    Dec(L.LemFrame);
    Dec(L.LemPhysicsFrame);

    if UserSetNuking and (L.LemExplosionTimer <= 0) and (Index_LemmingToBeNuked > L.LemIndex) then
      Transition(L, baOhnoing);
  end else
  if L.LemEndOfAnimation then RemoveLemming(L, RM_SAVE);
end;
    */

}