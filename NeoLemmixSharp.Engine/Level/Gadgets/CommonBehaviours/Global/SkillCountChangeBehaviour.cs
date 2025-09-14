using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;

public sealed class SkillCountChangeBehaviour : GadgetBehaviour
{
    private readonly LemmingSkill _lemmingSkill;
    private readonly int _overrideTribeId;
    public readonly int SkillCountDelta;

    public SkillCountChangeBehaviour(
        LemmingSkill lemmingSkill,
        int overrideTribeId,
        int skillCountDelta)
        : base(GadgetBehaviourType.GlobalSkillCountChange)
    {
        _lemmingSkill = lemmingSkill;
        _overrideTribeId = overrideTribeId;
        SkillCountDelta = skillCountDelta;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var tribe = GetTribe(lemmingId);

        LevelScreen.SkillSetManager.ChangeSkillCount(_lemmingSkill, tribe, SkillCountDelta);
    }

    private Tribe? GetTribe(int lemmingId)
    {
        if (_overrideTribeId == EngineConstants.SkillCountChangeBehaviourNoOverrideValue)
            return lemmingId == EngineConstants.NoLemmingCauseAndEffectId
                ? null
                : GetLemming(lemmingId).State.TribeAffiliation;

        return LevelScreen.TribeManager.GetTribeForId(_overrideTribeId);
    }
}
