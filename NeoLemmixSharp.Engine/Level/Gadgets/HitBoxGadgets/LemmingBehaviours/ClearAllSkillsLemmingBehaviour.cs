using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class ClearAllSkillsLemmingBehaviour : LemmingBehaviour
{
    public ClearAllSkillsLemmingBehaviour() : base(LemmingBehaviourType.ClearLemmingStates)
    {
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        lemming.State.ClearAllPermanentSkills();
    }
}
