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
    [FieldOffset(0 * sizeof(ushort))] private readonly ulong _allBits;

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
    public bool Equals(FileFormatVersion other) => _allBits == other._allBits;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FileFormatVersion other && Equals(other);

    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode()
    {
        var u1 = (int)_allBits;
        var u2 = (int)(_allBits >>> 32);
        var result = 3992171 * u1 +
                     7851007 * u2;
        return result;
    }

    [Pure]
    [DebuggerStepThrough]
    public int CompareTo(FileFormatVersion value)
    {
        return _allBits.CompareTo(value._allBits);
    }

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> charBuffer = stackalloc char[1 + (Helpers.Uint16NumberBufferLength * 4) + 3 + 1];
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
    public static bool operator <(FileFormatVersion v1, FileFormatVersion v2) => v1._allBits < v2._allBits;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator <=(FileFormatVersion v1, FileFormatVersion v2) => v1._allBits <= v2._allBits;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator >(FileFormatVersion v1, FileFormatVersion v2) => v1._allBits > v2._allBits;
    [Pure]
    [DebuggerStepThrough]
    public static bool operator >=(FileFormatVersion v1, FileFormatVersion v2) => v1._allBits >= v2._allBits;
}
