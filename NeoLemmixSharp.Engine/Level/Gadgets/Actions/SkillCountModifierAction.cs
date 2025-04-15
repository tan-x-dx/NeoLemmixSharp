using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SkillCountModifierAction : IGadgetAction
{
    private readonly LemmingSkill _skill;
    private readonly int _value;
    private readonly int? _teamId;
    private readonly bool _isDelta;
    public GadgetActionType ActionType => GadgetActionType.ChangeSkillCount;

    public SkillCountModifierAction(LemmingSkill skill, int value, int? teamId, bool isDelta)
    {
        _skill = skill;
        _value = value;
        _teamId = teamId;
        _isDelta = isDelta;
    }

    public void PerformAction(Lemming lemming)
    {
        var team = LevelScreen.TeamManager.GetTeamForId(_teamId);
        LevelScreen.SkillSetManager.SetSkillCount(_skill, team, _value, _isDelta);
    }
}