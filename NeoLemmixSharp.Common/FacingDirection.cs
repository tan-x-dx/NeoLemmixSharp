using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public SpriteEffects AsSpriteEffects() => (SpriteEffects)Id;

    public FacingDirection(int id)
    {
        Id = id & 1;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public FacingDirection GetOpposite() => new(Id + 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.Rotate((Id << 1) - 1);

    [Pure]
    int IIdEquatable<FacingDirection>.Id => Id;

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(FacingDirection other) => Id == other.Id;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FacingDirection other && Id == other.Id;
    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;
    [Pure]
    [DebuggerStepThrough]
    public override string ToString() => Id == EngineConstants.RightFacingDirectionId
        ? EngineConstants.RightFacingDirectionName
        : EngineConstants.LeftFacingDirectionName;

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(FacingDirection first, FacingDirection second) => first.Id == second.Id;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(FacingDirection first, FacingDirection second) => first.Id != second.Id;
}