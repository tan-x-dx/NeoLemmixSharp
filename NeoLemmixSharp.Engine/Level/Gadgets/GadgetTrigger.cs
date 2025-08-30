using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetTrigger : IIdEquatable<GadgetTrigger>
{
    public required GadgetBehaviour[] GadgetBehaviours { private get; init; }
    public required GadgetTriggerName TriggerName { get; init; }
    public required int Id { get; init; }
    public GadgetTriggerType TriggerType { get; }

    protected GadgetTrigger(GadgetTriggerType triggerType)
    {
        TriggerType = triggerType;
    }

    public ReadOnlySpan<GadgetBehaviour> Behaviours => new(GadgetBehaviours);

    public abstract void Tick();

    public bool Equals(GadgetTrigger? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => TriggerName.ToString();

    public static bool operator ==(GadgetTrigger left, GadgetTrigger right) => left.Id == right.Id;
    public static bool operator !=(GadgetTrigger left, GadgetTrigger right) => left.Id != right.Id;
}
