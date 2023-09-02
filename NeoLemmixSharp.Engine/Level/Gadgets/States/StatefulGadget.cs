using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class StatefulGadget : GadgetBase
{
    private readonly GadgetState[] _states;

    private int _currentStateIndex;

    public override GadgetType Type { get; }
    public override Orientation Orientation { get; }

    public GadgetState CurrentState => _states[_currentStateIndex];

    public StatefulGadget(
        int id,
        GadgetType type,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        GadgetState[] states)
        : base(id, gadgetBounds)
    {
        Type = type;
        Orientation = orientation;
        _states = states;
    }

    public void SetState(int stateIndex)
    {
        var previousState = CurrentState;

        _currentStateIndex = stateIndex;

        var currentState = CurrentState;

        previousState.OnTransitionFrom();
        currentState.OnTransitionTo();
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        throw new NotImplementedException();
    }

    public override bool CaresAboutLemmingInteraction => _states.Any(s => s.HitBoxBehaviour.InteractsWithLemming);
    public override bool MatchesLemming(Lemming lemming) => CurrentState.HitBoxBehaviour.MatchesLemming(lemming);
    public override void OnLemmingMatch(Lemming lemming)
    {
        
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => CurrentState.HitBoxBehaviour.MatchesPosition(levelPosition);

    public override void Tick()
    {
        CurrentState.Tick();
    }
}