using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SkillCountModifierAction : GadgetAction
{
    private readonly LemmingSkill _skill;
    private readonly int _value;
    private readonly int? _tribeId;
    private readonly bool _isDelta;

    public SkillCountModifierAction(LemmingSkill skill, int value, int? tribeId, bool isDelta)
        : base(GadgetActionType.ChangeSkillCount)
    {
        _skill = skill;
        _value = value;
        _tribeId = tribeId;
        _isDelta = isDelta;
    }

    public override void PerformAction(Lemming lemming)
    {
        var tribe = LevelScreen.TribeManager.GetTribeForId(_tribeId);
        LevelScreen.SkillSetManager.SetSkillCount(_skill, tribe, _value, _isDelta);
    }
}
