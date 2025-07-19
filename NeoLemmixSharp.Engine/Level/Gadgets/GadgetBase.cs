using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible<int>
{
    private readonly string _gadgetName;

    private GadgetState _currentState;
    private GadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public abstract GadgetState CurrentState { get; }

    public required GadgetBounds CurrentGadgetBounds { get; init; }

    public required int Id { get; init; }
    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool IsFastForward { get; init; }

    public Point Position => CurrentGadgetBounds.Position;
    public Size Size => CurrentGadgetBounds.Size;

    public GadgetBase(string gadgetName)
    {
        _gadgetName = gadgetName;
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
            CurrentState.Tick();
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

        _currentState = GetState(_currentStateIndex);

        _previousState.OnTransitionFrom();
        _currentState.OnTransitionTo();

        OnChangeStates(_currentStateIndex);
    }

    protected abstract GadgetState GetState(int stateIndex);

    protected abstract void OnChangeStates(int currentStateIndex);

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
