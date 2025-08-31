using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;

public sealed class SkillCountChangeBehaviour : GadgetBehaviour
{
    private readonly LemmingSkill _lemmingSkill;
    private readonly int _overrideTribeId;
    private readonly int _skillCountDelta;

    public SkillCountChangeBehaviour(
        LemmingSkill lemmingSkill,
        int overrideTribeId,
        int skillCountDelta)
        : base(GadgetBehaviourType.GlobalSkillCountChange)
    {
        _lemmingSkill = lemmingSkill;
        _overrideTribeId = overrideTribeId;
        _skillCountDelta = skillCountDelta;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var tribe = GetTribe(lemmingId);

        LevelScreen.SkillSetManager.ChangeSkillCount(_lemmingSkill, tribe, _skillCountDelta);
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
