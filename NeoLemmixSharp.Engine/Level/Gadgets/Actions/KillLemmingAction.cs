using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class KillLemmingAction : GadgetAction
{
    private readonly LemmingRemovalReason _lemmingRemovalReason;

    public KillLemmingAction(LemmingRemovalReason lemmingRemovalReason) : base(GadgetActionType.KillLemming)
    {
        _lemmingRemovalReason = lemmingRemovalReason;
    }

    public override void PerformAction(Lemming lemming)
    {
        LevelScreen.LemmingManager.RemoveLemming(lemming, _lemmingRemovalReason);
    }
}
