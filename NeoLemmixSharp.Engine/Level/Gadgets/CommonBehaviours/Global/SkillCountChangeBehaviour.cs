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

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var tribe = GetTribe(triggerData);

        LevelScreen.SkillSetManager.ChangeSkillCount(_lemmingSkill, tribe, SkillCountDelta);
    }

    private Tribe? GetTribe(int triggerData)
    {
        int? tribeId = _overrideTribeId;

        if (_overrideTribeId == EngineConstants.SkillCountChangeBehaviourNoOverrideValue)
        {
            tribeId = triggerData == -1
                ? null
                : GetLemming(triggerData).State.TribeId;
        }

        return LevelScreen.TribeManager.TryGetTribe(tribeId);
    }
}
