using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class LemmingSpecificSkillCountChangeBehaviour : LemmingBehaviour
{
    private readonly LemmingSkill _lemmingSkill;
    public readonly int SkillCountDelta;

    public LemmingSpecificSkillCountChangeBehaviour(
        LemmingSkill lemmingSkill,
        int skillCountDelta)
        : base(LemmingBehaviourType.SkillCountChange)
    {
        _lemmingSkill = lemmingSkill;
        SkillCountDelta = skillCountDelta;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        var tribeId = lemming.State.TribeId;

        LevelScreen.SkillSetManager.ChangeSkillCount(_lemmingSkill, tribeId, SkillCountDelta);
    }
}
