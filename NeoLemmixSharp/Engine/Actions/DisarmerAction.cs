namespace NeoLemmixSharp.Engine.Actions;

public sealed class DisarmerAction : LemmingAction
{
    public const int NumberOfDisarmerAnimationFrames = 16;

    public static DisarmerAction Instance { get; } = new();

    private DisarmerAction()
    {
    }

    public override int ActionId => 8;
    public override string LemmingActionName => "disarmer";
    public override int NumberOfAnimationFrames => NumberOfDisarmerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        lemming.DisarmingFrames--;
        if (lemming.DisarmingFrames <= 0)
        {
            if (lemming.NextAction == NoneAction.Instance)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.NextAction.TransitionLemmingToAction(lemming, false);
                lemming.NextAction = NoneAction.Instance;
            }
        }
        else if ((lemming.AnimationFrame & 7) == 0)
        {
            // ?? CueSoundEffect(SFX_FIXING, L.Position); ??
        }

        return false;
    }
}