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

    public Gadget(
        int id,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        GadgetState[] states)
    {
        _states = states;

        Id = id;
        Orientation = orientation;
        GadgetBounds = gadgetBounds;
    }

    public void SetState(int stateIndex)
    {
        var previousState = CurrentState;

        _currentStateIndex = stateIndex;

        var currentState = CurrentState;

        previousState.OnTransitionFrom();
        currentState.OnTransitionTo();
    }

    public bool MatchesLemmingAndPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return false;
    }

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