﻿using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ExiterAction : LemmingAction
{
    public static readonly ExiterAction Instance = new();

    private ExiterAction()
    {
    }

    public override int Id => LevelConstants.ExiterActionId;
    public override string LemmingActionName => "exiter";
    public override int NumberOfAnimationFrames => LevelConstants.ExiterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.CountDownTimer = 0;
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