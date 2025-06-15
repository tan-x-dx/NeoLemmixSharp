using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SkillCountModifierAction : GadgetAction
{
    private readonly LemmingSkill _skill;
    private readonly int _value;
    private readonly int? _tribeId;

    public SkillCountModifierAction(LemmingSkill skill, int value, int? tribeId)
        : base(GadgetActionType.ChangeSkillCount)
    {
        _skill = skill;
        _value = value;
        _tribeId = tribeId;
    }

    public override void PerformAction(Lemming lemming)
    {
        var tribe = LevelScreen.TribeManager.GetTribeForId(_tribeId);
        LevelScreen.SkillSetManager.SetSkillCount(_skill, tribe, _value);
    }
}
