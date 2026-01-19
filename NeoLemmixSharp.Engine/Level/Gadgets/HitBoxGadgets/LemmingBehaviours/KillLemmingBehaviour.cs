using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

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

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        LevelScreen.LemmingManager.RemoveLemming(lemming, _lemmingRemovalReason);
    }
}
