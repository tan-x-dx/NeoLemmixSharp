using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(ushort))]
public readonly struct FileFormatVersion : IComparable<FileFormatVersion>, IEquatable<FileFormatVersion>
{
    [FieldOffset(0 * sizeof(ushort))] private readonly uint _lowerBits;
    [FieldOffset(2 * sizeof(ushort))] private readonly uint _upperBits;

    [FieldOffset(0 * sizeof(ushort))] public readonly ushort Revision;
    [FieldOffset(1 * sizeof(ushort))] public readonly ushort Build;
    [FieldOffset(2 * sizeof(ushort))] public readonly ushort Minor;
    [FieldOffset(3 * sizeof(ushort))] public readonly ushort Major;

    public FileFormatVersion(ushort major, ushort minor, ushort build, ushort revision)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Revision = revision;
    }

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(FileFormatVersion other) => _upperBits == other._upperBits &&
                                                   _lowerBits == other._lowerBits;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FileFormatVersion other && Equals(other);

    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode()
    {
        var result = 3992171 * _upperBits +
                     7851007 * _lowerBits;
        return (int)result;
    }

    [Pure]
    [DebuggerStepThrough]
    public int CompareTo(FileFormatVersion value)
    {
        return _upperBits != value._upperBits ? _upperBits > value._upperBits ? 1 : -1 :
               _lowerBits != value._lowerBits ? _lowerBits > value._lowerBits ? 1 : -1 :
               0;
    }

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> charBuffer = stackalloc char[1 + Helpers.Uint16NumberBufferLength * 4 + 3 + 1];
        TryFormat(charBuffer, out var charsWritten);
        return charBuffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        ReadOnlySpan<int> components = [Major, Minor, Build, Revision];
        var formatParameters = new Helpers.FormatParameters('[', '.', ']');
        return Helpers.TryFormatSpan(components, destination, formatParameters, out charsWritten);
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(FileFormatVersion left, FileFormatVersion right) => left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(FileFormatVersion left, FileFormatVersion right) => !left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator <(FileFormatVersion v1, FileFormatVersion v2) => v1.CompareTo(v2) < 0;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator <=(FileFormatVersion v1, FileFormatVersion v2) => v1.CompareTo(v2) <= 0;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator >(FileFormatVersion v1, FileFormatVersion v2) => v1.CompareTo(v2) > 0;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator >=(FileFormatVersion v1, FileFormatVersion v2) => v1.CompareTo(v2) >= 0;
}
