using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util;

[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(ushort))]
public readonly struct FileVersion : IComparable<FileVersion>, IEquatable<FileVersion>
{
    [FieldOffset(0 * sizeof(ushort))] private readonly uint _upperBits;
    [FieldOffset(2 * sizeof(ushort))] private readonly uint _lowerBits;

    [FieldOffset(0 * sizeof(ushort))] public readonly ushort Major;
    [FieldOffset(1 * sizeof(ushort))] public readonly ushort Minor;
    [FieldOffset(2 * sizeof(ushort))] public readonly ushort Build;
    [FieldOffset(3 * sizeof(ushort))] public readonly ushort Revision;

    public FileVersion(ushort major, ushort minor, ushort build, ushort revision)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Revision = revision;
    }

    public bool Equals(FileVersion other) => _upperBits == other._upperBits &&
                                             _lowerBits == other._lowerBits;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FileVersion other && Equals(other);

    public override int GetHashCode()
    {
        var result = 3992171 * _upperBits +
                     7851007 * _lowerBits +
                     9472289;
        return (int)result;
    }

    public int CompareTo(FileVersion value)
    {
        return _upperBits != value._upperBits ? (_upperBits > value._upperBits ? 1 : -1) :
               _lowerBits != value._lowerBits ? (_lowerBits > value._lowerBits ? 1 : -1) :
               0;
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

    public static bool operator ==(FileVersion left, FileVersion right) => left.Equals(right);
    public static bool operator !=(FileVersion left, FileVersion right) => !left.Equals(right);
    public static bool operator <(FileVersion v1, FileVersion v2) => v1.CompareTo(v2) < 0;
    public static bool operator <=(FileVersion v1, FileVersion v2) => v1.CompareTo(v2) <= 0;
    public static bool operator >(FileVersion v1, FileVersion v2) => v1.CompareTo(v2) > 0;
    public static bool operator >=(FileVersion v1, FileVersion v2) => v1.CompareTo(v2) >= 0;
}
