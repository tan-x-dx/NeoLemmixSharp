using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class StatefulGadget : GadgetBase, IMoveableGadget
{
    private readonly Dictionary<string, IGadgetInput> _inputLookup = new();
    private readonly HitBox _hitBox;
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
        HitBox hitBox,
        GadgetState[] states)
        : base(id, gadgetBounds)
    {
        Type = type;
        Orientation = orientation;
        _hitBox = hitBox;
        _states = states;
    }

    public void RegisterGadgetInput(IGadgetInput gadgetInput)
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

    public override bool CaresAboutLemmingInteraction => true;
    public override bool MatchesLemming(Lemming lemming) => _hitBox.MatchesLemming(lemming);
    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return _hitBox.MatchesLemmingData(lemming) &&
               _hitBox.MatchesPosition(levelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        var actions = CurrentState.Actions;
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
        CurrentState.Tick();
    }
}