using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class ClearAllSkillsLemmingBehaviour : LemmingBehaviour
{
    public ClearAllSkillsLemmingBehaviour() : base(LemmingBehaviourType.ClearLemmingStates)
    {
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        lemming.State.ClearAllPermanentSkills();
    }
}
