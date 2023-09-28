using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class SetLemmingStateBehaviour : IGadgetBehaviour
{
    private readonly ILemmingStateChanger _lemmingStateChanger;
    private readonly SetPermanentSkillBehaviourType _type;

    public SetLemmingStateBehaviour(ILemmingStateChanger lemmingStateChanger, SetPermanentSkillBehaviourType type)
    {
        _lemmingStateChanger = lemmingStateChanger;
        _type = type;
    }

    public void PerformAction(Lemming lemming)
    {
        var lemmingState = lemming.State;

        if (_type == SetPermanentSkillBehaviourType.Toggle)
        {
            _lemmingStateChanger.ToggleLemmingState(lemmingState);
            return;
        }

        _lemmingStateChanger.SetLemmingState(lemmingState, _type != SetPermanentSkillBehaviourType.Clear);
    }

    public enum SetPermanentSkillBehaviourType
    {
        Clear,
        Set,
        Toggle
    }
}

public sealed class ZombieStateChanger : ILemmingStateChanger
{
    public static ZombieStateChanger Instance { get; } = new();

    private ZombieStateChanger()
    {
    }

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsZombie = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        var isZombie = lemmingState.IsZombie;
        lemmingState.IsZombie = !isZombie;
    }
}

public sealed class NeutralStateChanger : ILemmingStateChanger
{
    public static NeutralStateChanger Instance { get; } = new();

    private NeutralStateChanger()
    {
    }

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsNeutral = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        var isNeutral = lemmingState.IsNeutral;
        lemmingState.IsNeutral = !isNeutral;
    }
}