using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetTrigger : IIdEquatable<GadgetTrigger>
{
    private readonly int[] _gadgetBehaviourIds;

    public required GadgetTriggerName TriggerName { get; init; }
    public required int Id { get; init; }

    public ReadOnlySpan<int> BehaviourIds => new(_gadgetBehaviourIds);

    protected GadgetTrigger(
        int[] gadgetBehaviourIds)
    {
        _gadgetBehaviourIds = gadgetBehaviourIds;
    }

    public abstract void Tick();

    public bool Equals(GadgetTrigger? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => TriggerName.ToString();

    public static bool operator ==(GadgetTrigger left, GadgetTrigger right) => left.Id == right.Id;
    public static bool operator !=(GadgetTrigger left, GadgetTrigger right) => left.Id != right.Id;
}
