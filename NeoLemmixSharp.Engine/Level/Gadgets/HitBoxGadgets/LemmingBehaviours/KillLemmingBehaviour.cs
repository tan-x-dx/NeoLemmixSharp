using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class KillLemmingBehaviour : LemmingBehaviour
{
    private readonly LemmingRemovalReason _lemmingRemovalReason;

    public KillLemmingBehaviour(
        LemmingRemovalReason lemmingRemovalReason)
        : base(LemmingBehaviourType.KillLemming)
    {
        _lemmingRemovalReason = lemmingRemovalReason;
    }

    public override void PerformBehaviour(Lemming lemming)
    {
        LevelScreen.LemmingManager.RemoveLemming(lemming, _lemmingRemovalReason);
    }
}
