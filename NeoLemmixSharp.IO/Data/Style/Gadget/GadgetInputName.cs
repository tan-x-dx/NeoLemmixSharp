using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct GadgetInputName : IEquatable<GadgetInputName>
{
    private readonly string _inputName;

    public GadgetInputName(string? inputName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputName);

        _inputName = inputName;
    }

    public override string ToString() => _inputName;

    public bool Equals(GadgetInputName other) => string.Equals(_inputName, other._inputName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetInputName other && Equals(other);
    public override int GetHashCode() => _inputName.GetHashCode();
    public static bool operator ==(GadgetInputName left, GadgetInputName right) => left.Equals(right);
    public static bool operator !=(GadgetInputName left, GadgetInputName right) => !left.Equals(right);
}
