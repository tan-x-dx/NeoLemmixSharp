using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public readonly struct FacingDirection : IIdEquatable<FacingDirection>
{
    public static readonly FacingDirection Right = new(EngineConstants.RightFacingDirectionId);
    public static readonly FacingDirection Left = new(EngineConstants.LeftFacingDirectionId);

    public readonly int Id;
    [Pure]
    public int DeltaX => 1 - (Id << 1);

    public FacingDirection(int id)
    {
        Id = id & 1;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FacingDirection GetOpposite() => new(Id + 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.Rotate((Id << 1) - 1);

    [Pure]
    int IIdEquatable<FacingDirection>.Id => Id;

    [Pure]
    public bool Equals(FacingDirection other) => Id == other.Id;
    [Pure]
    public override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    [Pure]
    public override int GetHashCode() => Id;
    [Pure]
    public override string ToString() => Id == EngineConstants.RightFacingDirectionId
        ? EngineConstants.RightFacingDirectionName
        : EngineConstants.LeftFacingDirectionName;

    [Pure]
    public static bool operator ==(FacingDirection first, FacingDirection second) => first.Id == second.Id;
    [Pure]
    public static bool operator !=(FacingDirection first, FacingDirection second) => first.Id != second.Id;
}