using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class ClearAllSkillsLemmingBehaviour : LemmingBehaviour
{
    public ClearAllSkillsLemmingBehaviour() : base(LemmingBehaviourType.ClearLemmingStates)
    {
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        lemming.State.ClearAllPermanentSkills();
    }
}
