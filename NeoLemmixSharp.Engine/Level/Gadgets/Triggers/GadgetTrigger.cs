using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GadgetTrigger : IEquatable<GadgetTrigger>
{
    private readonly GeneralBehaviour[] _gadgetBehaviours;

    public GadgetTriggerName TriggerName { get; }

    public ReadOnlySpan<GeneralBehaviour> Behaviours => new(_gadgetBehaviours);

    protected GadgetTrigger(GadgetTriggerName triggerName, GeneralBehaviour[] gadgetBehaviours)
    {
        TriggerName = triggerName;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public abstract void OnNewTick();

    protected void ResetGeneralBehaviours()
    {
        foreach (var behaviour in _gadgetBehaviours)
        {
            behaviour.Reset();
        }
    }

    public bool Equals(GadgetTrigger? other) => other is not null && TriggerName.Equals(other.TriggerName);
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && TriggerName.Equals(other.TriggerName);
    public sealed override int GetHashCode() => TriggerName.GetHashCode();
    public sealed override string ToString() => TriggerName.ToString();
}
