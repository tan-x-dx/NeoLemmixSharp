using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class LemmingSpecificSkillCountChangeBehaviour : LemmingBehaviour
{
    private readonly LemmingSkillType _lemmingSkill;
    public readonly int SkillCountDelta;

    public LemmingSpecificSkillCountChangeBehaviour(
        LemmingSkillType lemmingSkill,
        int skillCountDelta)
        : base(LemmingBehaviourType.SkillCountChange)
    {
        _lemmingSkill = lemmingSkill;
        SkillCountDelta = skillCountDelta;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        var tribeId = lemming.TribeId;

        LevelScreen.SkillSetManager.ChangeSkillCount(_lemmingSkill, tribeId, SkillCountDelta);
    }
}
