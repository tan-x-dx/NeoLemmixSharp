using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

public readonly struct GadgetTriggerName : IEquatable<GadgetTriggerName>
{
    private readonly string _inputName;

    public GadgetTriggerName(string? inputName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputName);

        _inputName = inputName;
    }

    public override string ToString() => _inputName;

    public bool Equals(GadgetTriggerName other) => string.Equals(_inputName, other._inputName, StringComparison.Ordinal);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTriggerName other && Equals(other);
    public override int GetHashCode() => _inputName.GetHashCode();
    public static bool operator ==(GadgetTriggerName left, GadgetTriggerName right) => left.Equals(right);
    public static bool operator !=(GadgetTriggerName left, GadgetTriggerName right) => !left.Equals(right);

    public static implicit operator GadgetTriggerName(string? gadgetStateName) => new(gadgetStateName);
}
