﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBehaviour : IIdEquatable<GadgetBehaviour>
{
    private readonly int _maxTriggerCountPerTick;
    public required GadgetBehaviourName GadgetBehaviourName { get; init; }
    public required int Id { get; init; }
    public GadgetBehaviourType BehaviourType { get; }

    private int _currentTickTriggerCount;

    public required int MaxTriggerCountPerTick
    {
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            _maxTriggerCountPerTick = value == 0
                ? EngineConstants.TrivialBehaviourTriggerLimit
                : value;
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

    public void PerformBehaviour(int lemmingId)
    {
        if (HasReachedMaxTriggerCount())
            return;

        PerformInternalBehaviour(lemmingId);
        _currentTickTriggerCount++;
    }

    protected abstract void PerformInternalBehaviour(int lemmingId);

    public bool Equals(GadgetBehaviour? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBehaviour other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => GadgetBehaviourName.ToString();

    public static bool operator ==(GadgetBehaviour left, GadgetBehaviour right) => left.Id == right.Id;
    public static bool operator !=(GadgetBehaviour left, GadgetBehaviour right) => left.Id != right.Id;

    protected static Lemming GetLemming(int lemmingId) => LevelScreen.LemmingManager.AllLemmings[lemmingId];
}
