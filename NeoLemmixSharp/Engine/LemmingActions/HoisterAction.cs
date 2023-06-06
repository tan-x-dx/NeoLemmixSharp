namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class HoisterAction : LemmingAction
{
    public const int NumberOfHoisterAnimationFrames = 8;

    public static HoisterAction Instance { get; } = new();

    private HoisterAction()
    {
    }

    public override int ActionId => 16;
    public override string LemmingActionName => "hoister";
    public override int NumberOfAnimationFrames => NumberOfHoisterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
        else if (lemming.AnimationFrame == 1 && lemming.IsStartingAction)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
        }
        else if (lemming.AnimationFrame <= 4)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 2);
        }

        return true;
    }

    /*
    function TLemmingGame.HandleHoisting(L: TLemming): Boolean;
begin
  Result := True;
  if L.LemEndOfAnimation then
    Transition(L, baWalking)
  // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
  else if (L.LemPhysicsFrame = 1) and L.LemIsStartingAction then
    Dec(L.LemY, 1)
  else if L.LemPhysicsFrame <= 4 then
    Dec(L.LemY, 2);
end;

    */

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var previouslyStartingAction = lemming.IsStartingAction;
        base.TransitionLemmingToAction(lemming, turnAround);
        lemming.IsStartingAction = previouslyStartingAction;
    }
}