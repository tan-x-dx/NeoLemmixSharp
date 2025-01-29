using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public readonly struct FacingDirection : IExtendedEnumType<FacingDirection>
{
    public static readonly FacingDirection Right = new(EngineConstants.RightFacingDirectionId);
    public static readonly FacingDirection Left = new(EngineConstants.LeftFacingDirectionId);

    public static int NumberOfItems => EngineConstants.NumberOfFacingDirections;
    private static ReadOnlySpan<int> RawInts =>
    [
        EngineConstants.RightFacingDirectionId,
        EngineConstants.LeftFacingDirectionId
    ];
    public static ReadOnlySpan<FacingDirection> AllItems => MemoryMarshal.Cast<int, FacingDirection>(RawInts);

    public readonly int Id;

    public int DeltaX => 1 - (Id << 1);

    public FacingDirection(int id)
    {
        Id = id & 1;
    }

    public FacingDirection(bool faceLeft)
    {
        Id = faceLeft
            ? EngineConstants.LeftFacingDirectionId
            : EngineConstants.RightFacingDirectionId;
    }

    [Pure]
    public FacingDirection GetOpposite() => new(Id + 1);

    [Pure]
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