namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ExiterAction : LemmingAction
{
    public const int NumberOfExiterAnimationFrames = 8;

    public static ExiterAction Instance { get; } = new();

    private ExiterAction()
    {
    }

    public override int Id => 25;
    public override string LemmingActionName => "exiter";
    public override int NumberOfAnimationFrames => NumberOfExiterAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
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