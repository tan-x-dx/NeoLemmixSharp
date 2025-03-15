using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public readonly struct FacingDirection : IIdEquatable<FacingDirection>
{
    public static readonly FacingDirection Right = new(EngineConstants.RightFacingDirectionId);
    public static readonly FacingDirection Left = new(EngineConstants.LeftFacingDirectionId);

    public readonly int Id;
    public int DeltaX => 1 - (Id << 1);

    public FacingDirection(int id)
    {
        Id = id & 1;
    }

    public FacingDirection(bool faceLeft)
    {
        if (faceLeft)
        {
            Id = EngineConstants.LeftFacingDirectionId;
        }
        else
        {
            Id = EngineConstants.RightFacingDirectionId;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FacingDirection GetOpposite() => new(Id + 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.Rotate((Id << 1) - 1);

    int IIdEquatable<FacingDirection>.Id => Id;

    public bool Equals(FacingDirection other) => Id == other.Id;
    public override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public override int GetHashCode() => Id;
    public override string ToString() => Id == EngineConstants.RightFacingDirectionId
        ? EngineConstants.RightFacingDirectionName
        : EngineConstants.LeftFacingDirectionName;

    public static bool operator ==(FacingDirection first, FacingDirection second) => first.Id == second.Id;
    public static bool operator !=(FacingDirection first, FacingDirection second) => first.Id != second.Id;
}