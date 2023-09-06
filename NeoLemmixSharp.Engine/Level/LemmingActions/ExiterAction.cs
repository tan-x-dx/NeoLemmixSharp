using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

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
    public override int CursorSelectionPriorityValue => GameConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    public override LevelPosition GetAnchorPosition() => new(2, 16);

    protected override int TopLeftBoundsDeltaX() => -2;
    protected override int TopLeftBoundsDeltaY() => 10;

    protected override int BottomRightBoundsDeltaX() => 3;

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