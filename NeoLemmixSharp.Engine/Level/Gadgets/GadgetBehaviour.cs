using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBehaviour : IIdEquatable<GadgetBehaviour>
{
    protected GadgetBase ParentGadget = null!;

    private readonly int _maxTriggerCountPerTick;
    public required GadgetBehaviourName GadgetBehaviourName { get; init; }
    public required int Id { get; init; }
    public GadgetBehaviourType BehaviourType { get; }

    private int _currentTickTriggerCount;

    public void SetParentGadget(GadgetBase gadget)
    {
        Debug.Assert(ParentGadget is null);

        ParentGadget = gadget;
    }

    public required int MaxTriggerCountPerTick
    {
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            _maxTriggerCountPerTick = value > 0
                ? value
                : EngineConstants.TrivialBehaviourTriggerLimit;
        }
    }

    protected GadgetBehaviour(GadgetBehaviourType behaviourType)
    {
        BehaviourType = behaviourType;
    }

    public void Reset()
    {
        _currentTickTriggerCount = 0;
        OnReset();
    }

    protected virtual void OnReset() { }

    private bool HasReachedMaxTriggerCount() => _currentTickTriggerCount >= _maxTriggerCountPerTick;

    public void PerformBehaviour(int triggerData)
    {
        if (HasReachedMaxTriggerCount())
            return;

        PerformInternalBehaviour(triggerData);
        _currentTickTriggerCount++;
    }

    protected abstract void PerformInternalBehaviour(int triggerData);

    public bool Equals(GadgetBehaviour? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBehaviour other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => GadgetBehaviourName.ToString();

    public static bool operator ==(GadgetBehaviour left, GadgetBehaviour right) => left.Id == right.Id;
    public static bool operator !=(GadgetBehaviour left, GadgetBehaviour right) => left.Id != right.Id;

    protected static Lemming GetLemming(int lemmingId) => LevelScreen.LemmingManager.AllLemmings[lemmingId];
}
