using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

public sealed class SkillCountModifierBehaviour : GadgetBehaviour
{
    private readonly LemmingSkill _skill;
    private readonly int _value;
    private readonly int? _tribeId;

    public SkillCountModifierBehaviour(
        int maxTriggerCountPerUpdate,
        LemmingSkill skill,
        int value,
        int? tribeId)
        : base(maxTriggerCountPerUpdate)
    {
        _skill = skill;
        _value = value;
        _tribeId = tribeId;
    }

    protected override void PerformInternalBehaviour()
    {
        var tribe = LevelScreen.TribeManager.GetTribeForId(_tribeId);
        LevelScreen.SkillSetManager.SetSkillCount(_skill, tribe, _value);
    }
}
