using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class Gadget : IIdEquatable<Gadget>
{
    private readonly GadgetState[] _states;

    private int _currentStateIndex;

    public int Id { get; }
    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public RectangularLevelRegion GadgetBounds { get; }

    public GadgetState CurrentState => _states[_currentStateIndex];
    public bool AnyStatesCareAboutLemmingInteraction => _states.Any(s => s.HitBoxBehaviour.InteractsWithLemming);

    public Gadget(
        int id,
        GadgetType gadgetType,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        GadgetState[] states)
    {
        Id = id;
        Type = gadgetType;
        Orientation = orientation;
        GadgetBounds = gadgetBounds;

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

    public bool MatchesLemming(Lemming lemming) => CurrentState.HitBoxBehaviour.MatchesLemming(lemming);
    public bool MatchesPosition(LevelPosition levelPosition) => CurrentState.HitBoxBehaviour.MatchesPosition(levelPosition);

    public void Tick()
    {
        CurrentState.Tick();
    }

    public bool Equals(Gadget? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Gadget other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Gadget left, Gadget right) => left.Id == right.Id;
    public static bool operator !=(Gadget left, Gadget right) => left.Id != right.Id;
}