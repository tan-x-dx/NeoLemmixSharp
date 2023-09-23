using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class SetPermanentSkillBehaviour : IGadgetBehaviour
{
    private readonly IPermanentSkill _permanentSkill;
    private readonly SetPermanentSkillBehaviourType _type;

    public SetPermanentSkillBehaviour(IPermanentSkill permanentSkill, SetPermanentSkillBehaviourType type)
    {
        _permanentSkill = permanentSkill;
        _type = type;
    }

    public void PerformAction(Lemming lemming)
    {
        if (_type == SetPermanentSkillBehaviourType.Toggle)
        {
            _permanentSkill.TogglePermanentSkill(lemming);
            return;
        }

        _permanentSkill.SetPermanentSkill(lemming, _type != SetPermanentSkillBehaviourType.Clear);
    }

    public enum SetPermanentSkillBehaviourType
    {
        Clear,
        Set,
        Toggle
    }
}