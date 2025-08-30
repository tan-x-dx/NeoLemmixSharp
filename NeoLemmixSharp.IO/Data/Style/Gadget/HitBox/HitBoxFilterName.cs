using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public readonly struct HitBoxFilterName
{
    private readonly string _hitBoxFilterName;

    public HitBoxFilterName(string? gadgetName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gadgetName);

        _hitBoxFilterName = gadgetName;
    }

    public GadgetTriggerName ToTriggerName() => new(_hitBoxFilterName);

    public override string ToString() => _hitBoxFilterName;

    public bool Equals(HitBoxFilterName other) => string.Equals(_hitBoxFilterName, other._hitBoxFilterName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is HitBoxFilterName other && Equals(other);
    public override int GetHashCode() => _hitBoxFilterName.GetHashCode();

    public static bool operator ==(HitBoxFilterName left, HitBoxFilterName right) => left.Equals(right);
    public static bool operator !=(HitBoxFilterName left, HitBoxFilterName right) => !left.Equals(right);
}
