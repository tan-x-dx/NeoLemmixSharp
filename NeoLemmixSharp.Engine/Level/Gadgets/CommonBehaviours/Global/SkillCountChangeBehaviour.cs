using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;

public sealed class SkillCountChangeBehaviour : GadgetBehaviour
{
    private readonly LemmingSkillType _lemmingSkill;
    private readonly int _tribeId;
    public readonly int SkillCountDelta;

    public SkillCountChangeBehaviour(
        LemmingSkillType lemmingSkill,
        int tribeId,
        int skillCountDelta)
        : base(GadgetBehaviourType.GlobalSkillCountChange)
    {
        _lemmingSkill = lemmingSkill;
        _tribeId = tribeId;
        SkillCountDelta = skillCountDelta;
    }

    protected override void PerformInternalBehaviour()
    {
        LevelScreen.SkillSetManager.ChangeSkillCount(_lemmingSkill, _tribeId, SkillCountDelta);
    }
}
