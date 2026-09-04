using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public static class FacingDirectionConstants
{
    public const int NumberOfFacingDirections = 2;

    public const int RightFacingDirectionId = 0;
    public const string RightFacingDirectionName = "Right";

    public const int LeftFacingDirectionId = 1;
    public const string LeftFacingDirectionName = "Left";
}

public readonly struct FacingDirection : IEquatable<FacingDirection>, ISpanFormattable
{
    private const int BitMask = 1;

    public static FacingDirection Right => new(FacingDirectionConstants.RightFacingDirectionId);
    public static FacingDirection Left => new(FacingDirectionConstants.LeftFacingDirectionId);

    public readonly int Id;
    [Pure]
    public int DeltaX => 1 - (Id << 1);

    [DebuggerStepThrough]
    public FacingDirection(int id)
    {
        Id = id & BitMask;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public FacingDirection GetOpposite() => new(Id ^ 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.Rotate((Id << 1) - 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public SpriteEffects AsSpriteEffects() => (SpriteEffects)Id;

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(FacingDirection other) => ((Id ^ other.Id) & BitMask) == 0;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FacingDirection other && Equals(other);
    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode() => Id & BitMask;

    [Pure]
    [DebuggerStepThrough]
    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString();
    [Pure]
    [DebuggerStepThrough]
    public override string ToString()
    {
        var result = FacingDirectionConstants.RightFacingDirectionName;

        if ((Id & BitMask) != FacingDirectionConstants.RightFacingDirectionId)
            result = FacingDirectionConstants.LeftFacingDirectionName;

        return result;
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var constString = ToString();
        if (destination.Length < constString.Length)
        {
            charsWritten = 0;
            return false;
        }

        constString.CopyTo(destination);
        charsWritten = constString.Length;
        return true;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(FacingDirection first, FacingDirection second) => first.Equals(second);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(FacingDirection first, FacingDirection second) => !first.Equals(second);
}
