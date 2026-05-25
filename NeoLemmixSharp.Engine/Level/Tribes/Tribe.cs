using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data.Style.Theme;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Tribes;

public sealed class Tribe : IEquatable<Tribe>
{
    public TribeStyleIdentifier TribeIdentifier { get; }
    public TribeColorData ColorData { get; }
    public int Id { get; }

    public Tribe(
        int id,
        LemmingSpriteBank spriteBank,
        TribeStyleIdentifier tribeIdentifier)
    {
        Id = id;
        ColorData = spriteBank.GetColorData(tribeIdentifier);
        TribeIdentifier = tribeIdentifier;
    }

    [DebuggerStepThrough]
    public bool Equals(Tribe? other)
    {
        var otherId = -1;
        if (other is not null) otherId = other.Id;
        return Id == otherId;
    }

    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Tribe other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;
    [DebuggerStepThrough]
    public static bool operator ==(Tribe? left, Tribe? right)
    {
        var leftId = -1;
        if (left is not null) leftId = left.Id;
        var rightId = -1;
        if (right is not null) rightId = right.Id;
        return leftId == rightId;
    }

    [DebuggerStepThrough]
    public static bool operator !=(Tribe? left, Tribe? right) => !(left == right);
}
