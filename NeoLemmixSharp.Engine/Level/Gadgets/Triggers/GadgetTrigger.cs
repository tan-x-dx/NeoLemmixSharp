using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GadgetTrigger : IEquatable<GadgetTrigger>
{
    public GadgetTriggerName TriggerName { get; }

    protected GadgetTrigger(GadgetTriggerName triggerName)
    {
        TriggerName = triggerName;
    }

    public virtual void OnRegistered() { }

    public bool Equals(GadgetTrigger? other) => other is not null && TriggerName.Equals(other.TriggerName);
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && TriggerName.Equals(other.TriggerName);
    public sealed override int GetHashCode() => TriggerName.GetHashCode();
    public sealed override string ToString() => TriggerName.ToString();
}
