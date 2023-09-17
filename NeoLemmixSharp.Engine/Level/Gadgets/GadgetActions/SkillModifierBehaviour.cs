using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class SkillModifierBehaviour : IGadgetBehaviour
{
    private static SkillSetManager SkillSetManager { get; set; } = null!;

    public static void SetSkillSetManager(SkillSetManager skillSetManager)
    {
        SkillSetManager = skillSetManager;
    }

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
        SkillSetManager.SetSkillCount(_skill, _team, _value, _isDelta);
    }
}