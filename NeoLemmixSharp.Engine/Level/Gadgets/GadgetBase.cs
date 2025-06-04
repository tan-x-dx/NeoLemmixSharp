using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible<int>
{
    private readonly string _gadgetName;
    private readonly GadgetState[] _states;

    private GadgetState _currentState;
    private GadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public GadgetState CurrentState => _currentState;
    public AnimationController CurrentAnimationController => _currentState.AnimationController;

    public required GadgetBounds CurrentGadgetBounds { protected get; init; }

    public required int Id { get; init; }
    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool IsFastForward { get; init; }

    public Point Position => CurrentGadgetBounds.Position;
    public Size Size => CurrentGadgetBounds.Size;

    public GadgetRenderer Renderer { get; internal set; }

    public GadgetBase(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex)
    {
        _gadgetName = gadgetName;
        _states = states;

        _currentStateIndex = initialStateIndex;
        _currentState = _states[initialStateIndex];
        _previousState = _currentState;
    }

    public void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public void Tick()
    {
        OnTick();

        if (_currentStateIndex == _nextStateIndex)
        {
            CurrentState.Tick(this);
        }
        else
        {
            ChangeStates();
        }
    }

    protected abstract void OnTick();

    protected void ChangeStates()
    {
        _currentStateIndex = _nextStateIndex;

        _previousState = _currentState;

        _currentState = _states[_currentStateIndex];

        _previousState.OnTransitionFrom();
        _currentState.OnTransitionTo();

        OnChangeStates();
    }

    protected abstract void OnChangeStates();

    protected void UpdatePreviousState()
    {
        _previousState = _currentState;
    }

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;

    public void WriteToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
    }

    public sealed override string ToString() => _gadgetName;
}
