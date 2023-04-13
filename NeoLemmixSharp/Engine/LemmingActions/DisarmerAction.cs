namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DisarmerAction : LemmingAction
{
    public const int NumberOfDisarmerAnimationFrames = 16;

    public static DisarmerAction Instance { get; } = new();

    private DisarmerAction()
    {
    }

    protected override int ActionId => 8;
    public override string LemmingActionName => "disarmer";
    public override int NumberOfAnimationFrames => NumberOfDisarmerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
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

        return false;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}