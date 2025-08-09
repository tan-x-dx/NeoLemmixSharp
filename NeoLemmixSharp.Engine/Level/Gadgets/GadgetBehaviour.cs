using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBehaviour : IIdEquatable<GadgetBehaviour>
{
    public required int Id { get; init; }
    public GadgetBehaviourType BehaviourType { get; }

    public required int MaxTriggerCountPerTick { get; init; }
    private int _currentTickTriggerCount;

    protected GadgetBehaviour(GadgetBehaviourType behaviourType)
    {
        BehaviourType = behaviourType;
    }

    public void Reset()
    {
        _currentTickTriggerCount = 0;
    }

    private bool HasReachedMaxTriggerCount() => _currentTickTriggerCount >= MaxTriggerCountPerTick;

    private void RegisterTrigger() => _currentTickTriggerCount++;

    public void PerformBehaviour()
    {
        if (HasReachedMaxTriggerCount())
            return;

        PerformInternalBehaviour();
        RegisterTrigger();
    }

    protected abstract void PerformInternalBehaviour();

    public bool Equals(GadgetBehaviour? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBehaviour other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBehaviour left, GadgetBehaviour right) => left.Id == right.Id;
    public static bool operator !=(GadgetBehaviour left, GadgetBehaviour right) => left.Id != right.Id;
}
