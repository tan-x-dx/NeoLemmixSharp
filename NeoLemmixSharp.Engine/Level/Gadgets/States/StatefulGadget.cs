using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class StatefulGadget : HitBoxGadget, IMoveableGadget
{
    private readonly Dictionary<string, IGadgetInput> _inputLookup = new();
    private readonly HitBox _hitBox;
    private readonly GadgetState[] _states;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public override GadgetType Type { get; }
    public override Orientation Orientation { get; }

    public StatefulGadget(
        int id,
        GadgetType type,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        HitBox hitBox,
        GadgetState[] states)
        : base(id, gadgetBounds)
    {
        Type = type;
        Orientation = orientation;
        _hitBox = hitBox;
        _states = states;

        foreach (var gadgetState in _states)
        {
            gadgetState.SetGadget(this);
        }
    }

    public void RegisterGadgetInput(StateSelectionInput gadgetInput)
    {
        _inputLookup.Add(gadgetInput.InputName, gadgetInput);

        gadgetInput.SetGadget(this);
    }

    public void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        return _inputLookup.TryGetValue(inputName, out var result) ? result : null;
    }

    public override bool MatchesLemming(Lemming lemming) => _hitBox.MatchesLemming(lemming);
    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return _hitBox.MatchesLemmingData(lemming) &&
               _hitBox.MatchesPosition(levelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        var actions = _states[_currentStateIndex].Actions;
        foreach (var action in actions)
        {
            action.PerformAction(lemming);
        }
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => _hitBox.MatchesPosition(levelPosition);

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
        if (_currentStateIndex != _nextStateIndex)
        {
            ChangeStates();
            return;
        }

        _states[_currentStateIndex].Tick();
    }

    private void ChangeStates()
    {
        var previousState = _states[_currentStateIndex];

        _currentStateIndex = _nextStateIndex;

        var currentState = _states[_currentStateIndex];

        previousState.OnTransitionFrom();
        currentState.OnTransitionTo();
    }

    public sealed class StateSelectionInput : IGadgetInput
    {
        private readonly int _stateIndex;
        public string InputName { get; }
        private StatefulGadget _gadget = null!;

        public StateSelectionInput(string inputName, int stateIndex)
        {
            InputName = inputName;
            _stateIndex = stateIndex;
        }

        public void SetGadget(StatefulGadget gadget)
        {
            _gadget = gadget;
        }

        public void ReactToSignal(bool signal)
        {
            _gadget.SetNextState(_stateIndex);
        }
    }
}