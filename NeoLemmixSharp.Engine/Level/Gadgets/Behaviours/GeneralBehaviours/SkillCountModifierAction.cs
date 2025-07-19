using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

public sealed class SkillCountModifierAction : GeneralBehaviour
{
    private readonly LemmingSkill _skill;
    private readonly int _value;
    private readonly int? _tribeId;

    public SkillCountModifierAction(LemmingSkill skill, int value, int? tribeId)
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
