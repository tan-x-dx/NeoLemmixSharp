using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ExiterAction : ILemmingAction
{
    public const int NumberOfExiterAnimationFrames = 8;

    public static ExiterAction Instance { get; } = new();

    private ExiterAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "exiter";
    public int NumberOfAnimationFrames => NumberOfExiterAnimationFrames;
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is ExiterAction;
    public override bool Equals(object? obj) => obj is ExiterAction;
    public override int GetHashCode() => nameof(ExiterAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
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

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}