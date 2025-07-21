using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GadgetTrigger : IEquatable<GadgetTrigger>
{
    private readonly GadgetBehaviour[] _gadgetBehaviours;

    public GadgetTriggerName TriggerName { get; }

    public ReadOnlySpan<GadgetBehaviour> Behaviours => new(_gadgetBehaviours);

    protected GadgetTrigger(GadgetTriggerName triggerName, GadgetBehaviour[] gadgetBehaviours)
    {
        TriggerName = triggerName;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public abstract void Tick();

    public bool Equals(GadgetTrigger? other) => other is not null && TriggerName.Equals(other.TriggerName);
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && TriggerName.Equals(other.TriggerName);
    public sealed override int GetHashCode() => TriggerName.GetHashCode();
    public sealed override string ToString() => TriggerName.ToString();
}
