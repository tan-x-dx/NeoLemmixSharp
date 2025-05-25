using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct GadgetInputData : IEquatable<GadgetInputData>
{
    private readonly string _inputName;

    public GadgetInputData(string? inputName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputName);

        _inputName = inputName;
    }

    public override string ToString() => _inputName;

    public bool Equals(GadgetInputData other) => string.Equals(_inputName, other._inputName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetInputData other && Equals(other);
    public override int GetHashCode() => _inputName.GetHashCode();
    public static bool operator ==(GadgetInputData left, GadgetInputData right) => left.Equals(right);
    public static bool operator !=(GadgetInputData left, GadgetInputData right) => !left.Equals(right);
}
