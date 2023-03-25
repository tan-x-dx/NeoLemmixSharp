using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DisarmerAction : ILemmingAction
{
    public const int NumberOfDisarmerAnimationFrames = 16;

    public static DisarmerAction Instance { get; } = new();

    private DisarmerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "disarmer";
    public int NumberOfAnimationFrames => NumberOfDisarmerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is DisarmerAction;
    public override bool Equals(object? obj) => obj is DisarmerAction;
    public override int GetHashCode() => nameof(DisarmerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        lemming.DisarmingFrames--;
        if (lemming.DisarmingFrames <= 0)
        {
            /* ??
            if L.LemActionNew <> baNone then Transition(L, L.LemActionNew)
            else Transition(L, baWalking);
            L.LemActionNew := baNone;
            */
        }
        else if ((lemming.AnimationFrame & 7) == 0)
        {
            // ?? CueSoundEffect(SFX_FIXING, L.Position); ??
        }
    }

    /*
        function TLemmingGame.HandleDisarming(L: TLemming): Boolean;
begin
  Result := False;
  Dec(L.LemDisarmingFrames);
  if L.LemDisarmingFrames <= 0 then
  begin
    if L.LemActionNew <> baNone then Transition(L, L.LemActionNew)
    else Transition(L, baWalking);
    L.LemActionNew := baNone;
  end
  else if L.LemPhysicsFrame mod 8 = 0 then
    CueSoundEffect(SFX_FIXING, L.Position);
end;
    */

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}