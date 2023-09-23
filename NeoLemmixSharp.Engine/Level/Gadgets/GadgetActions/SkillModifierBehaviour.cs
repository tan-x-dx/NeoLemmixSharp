using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class SkillModifierBehaviour : IGadgetBehaviour
{
    private readonly LemmingSkill _skill;
    private readonly Team? _team;
    private readonly int _value;
    private readonly bool _isDelta;

    public SkillModifierBehaviour(LemmingSkill skill, Team? team, int value, bool isDelta)
    {
        _skill = skill;
        _team = team;
        _value = value;
        _isDelta = isDelta;
    }

    public void PerformAction(Lemming lemming)
    {
        Global.SkillSetManager.SetSkillCount(_skill, _team, _value, _isDelta);
    }
}