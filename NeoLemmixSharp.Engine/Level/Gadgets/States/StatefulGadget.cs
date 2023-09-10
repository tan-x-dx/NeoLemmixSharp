using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class StatefulGadget : GadgetBase, IMoveableGadget
{
    private readonly Dictionary<string, IGadgetInput> _inputLookup = new();
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

    public void RegisterGadget(IGadgetInput gadgetInput)
    {
        _inputLookup.Add(gadgetInput.InputName, gadgetInput);
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
        return _inputLookup.TryGetValue(inputName, out var result) ? result : null;
    }

    public override bool CaresAboutLemmingInteraction => _states.Any(s => s.HitBoxBehaviour.InteractsWithLemming);
    public override bool MatchesLemming(Lemming lemming) => CurrentState.HitBoxBehaviour.MatchesLemming(lemming);
    public override void OnLemmingMatch(Lemming lemming)
    {
        CurrentState.HitBoxBehaviour.OnLemmingInHitBox(lemming);
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => CurrentState.HitBoxBehaviour.MatchesPosition(levelPosition);

    public void Move(int dx, int dy)
    {
        var newPosition = TopLeftPixel + new LevelPosition(dx, dy);

        UpdatePosition(newPosition);
    }

    public void SetPosition(int x, int y)
    {
        var newPosition = new LevelPosition(x, y);

        UpdatePosition(newPosition);
    }

    public override void Tick()
    {
        CurrentState.Tick();
    }
}