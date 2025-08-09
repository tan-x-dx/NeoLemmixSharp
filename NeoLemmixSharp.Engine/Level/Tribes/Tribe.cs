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
    public bool Equals(Tribe? other) => Id == (other?.Id ?? -1);
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Tribe other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;
    [DebuggerStepThrough]
    public static bool operator ==(Tribe? left, Tribe? right) => left?.Id == right?.Id;
    [DebuggerStepThrough]
    public static bool operator !=(Tribe? left, Tribe? right) => left?.Id != right?.Id;
}