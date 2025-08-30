using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

public readonly struct GadgetBehaviourName : IEquatable<GadgetBehaviourName>
{
    private readonly string _behaviourName;

    public GadgetBehaviourName(string? behaviourName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(behaviourName);

        _behaviourName = behaviourName;
    }

    public override string ToString() => _behaviourName;

    public bool Equals(GadgetBehaviourName other) => string.Equals(_behaviourName, other._behaviourName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBehaviourName other && Equals(other);
    public override int GetHashCode() => _behaviourName.GetHashCode();
    public static bool operator ==(GadgetBehaviourName left, GadgetBehaviourName right) => left.Equals(right);
    public static bool operator !=(GadgetBehaviourName left, GadgetBehaviourName right) => !left.Equals(right);
}
