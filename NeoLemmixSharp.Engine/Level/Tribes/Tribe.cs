using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data.Style.Theme;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Tribes;

public sealed class Tribe : IIdEquatable<Tribe>
{
    public readonly int Id;
    public readonly TribeColorData ColorData;
    public readonly TribeStyleIdentifier TribeIdentifier;

    public Tribe(
        int id,
        LemmingSpriteBank spriteBank,
        TribeStyleIdentifier tribeIdentifier)
    {
        Id = id;
        ColorData = spriteBank.GetColorData(tribeIdentifier);
        TribeIdentifier = tribeIdentifier;
    }

    int IIdEquatable<Tribe>.Id => Id;
    [DebuggerStepThrough]
    public bool Equals(Tribe? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Tribe other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;
    [DebuggerStepThrough]
    public static bool operator ==(Tribe? left, Tribe? right)
    {
        var leftValue = -1;
        if (left is not null) leftValue = left.Id;
        var rightValue = -1;
        if (right is not null) rightValue = right.Id;
        return leftValue == rightValue;
    }

    [DebuggerStepThrough]
    public static bool operator !=(Tribe? left, Tribe? right) => !(left == right);
}
