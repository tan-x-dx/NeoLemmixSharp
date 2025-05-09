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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public SpriteEffects AsSpriteEffects() => (SpriteEffects)Id;

    [Pure]
    int IIdEquatable<FacingDirection>.Id => Id;

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(FacingDirection other) => this == other;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FacingDirection other && this == other;
    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;
    [Pure]
    [DebuggerStepThrough]
    public override string ToString()
    {
        ReadOnlySpan<string> FacingDirectionNames =
        [
            EngineConstants.RightFacingDirectionName,
            EngineConstants.LeftFacingDirectionName
        ];

        return FacingDirectionNames[Id & 1];
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var constString = ToString();
        if (destination.Length < constString.Length)
        {
            charsWritten = 0;
            return false;
        }

        constString.AsSpan().CopyTo(destination);
        charsWritten = constString.Length;
        return true;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(FacingDirection first, FacingDirection second) => first.Id == second.Id;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(FacingDirection first, FacingDirection second) => first.Id != second.Id;
}