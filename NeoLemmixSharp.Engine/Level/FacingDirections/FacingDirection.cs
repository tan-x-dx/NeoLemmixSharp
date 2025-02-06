using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public readonly struct FacingDirection : IIdEquatable<FacingDirection>
{
    public static readonly FacingDirection Right = new(EngineConstants.RightFacingDirectionId);
    public static readonly FacingDirection Left = new(EngineConstants.LeftFacingDirectionId);

    public static int NumberOfItems => EngineConstants.NumberOfFacingDirections;
    private static ReadOnlySpan<int> RawInts =>
    [
        EngineConstants.RightFacingDirectionId, EngineConstants.RightFacingDirectionDeltaX,
        EngineConstants.LeftFacingDirectionId, EngineConstants.LeftFacingDirectionDeltaX
    ];
    public static ReadOnlySpan<FacingDirection> AllItems => MemoryMarshal.Cast<int, FacingDirection>(RawInts);

    public readonly int Id;
    public readonly int DeltaX;

    public FacingDirection()
    {
        Id = EngineConstants.RightFacingDirectionId;
        DeltaX = EngineConstants.RightFacingDirectionDeltaX;
    }

    public FacingDirection(int id)
    {
        Id = id & 1;
        DeltaX = EngineConstants.RightFacingDirectionDeltaX;
        if (Id == EngineConstants.LeftFacingDirectionId)
            DeltaX = EngineConstants.LeftFacingDirectionDeltaX;
    }

    public FacingDirection(bool faceLeft)
    {
        if (faceLeft)
        {
            Id = EngineConstants.LeftFacingDirectionId;
            DeltaX = EngineConstants.LeftFacingDirectionDeltaX;
        }
        else
        {
            Id = EngineConstants.RightFacingDirectionId;
            DeltaX = EngineConstants.RightFacingDirectionDeltaX;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FacingDirection GetOpposite() => new(Id + 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.Rotate(-DeltaX);

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