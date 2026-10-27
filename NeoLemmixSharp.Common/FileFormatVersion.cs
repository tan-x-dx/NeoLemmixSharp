using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(ushort))]
public readonly struct FileFormatVersion :
    ISpanFormattable,
    IEquatable<FileFormatVersion>,
    IEqualityOperators<FileFormatVersion, FileFormatVersion, bool>,
    IComparable<FileFormatVersion>,
    IComparisonOperators<FileFormatVersion, FileFormatVersion, bool>
{
    [FieldOffset(0 * sizeof(ushort))] public readonly ushort Revision;
    [FieldOffset(1 * sizeof(ushort))] public readonly ushort Build;
    [FieldOffset(2 * sizeof(ushort))] public readonly ushort Minor;
    [FieldOffset(3 * sizeof(ushort))] public readonly ushort Major;

    [DebuggerStepThrough]
    public FileFormatVersion(ushort major, ushort minor, ushort build, ushort revision)
    {
        Revision = revision;
        Build = build;
        Minor = minor;
        Major = major;
    }

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(FileFormatVersion other) => this == other;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FileFormatVersion other && this == other;

    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode()
    {
        var longValue = this.GetBitsAsUlong();

        var u1 = (int)longValue;
        var u2 = (int)(longValue >>> 32);
        var result = 3992171 * u1 +
                     7851007 * u2;
        return result;
    }

    [Pure]
    [DebuggerStepThrough]
    public int CompareTo(FileFormatVersion value) => Compare(this, value);

    [Pure]
    [DebuggerStepThrough]
    public static int Compare(FileFormatVersion left, FileFormatVersion right)
    {
        var leftLong = left.GetBitsAsUlong();
        var rightLong = right.GetBitsAsUlong();

        var gt = (leftLong > rightLong) ? 1 : 0;
        var lt = (leftLong < rightLong) ? 1 : 0;
        return gt - lt;
    }

    [Pure]
    [DebuggerStepThrough]
    [SkipLocalsInit]
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> charBuffer = stackalloc char[1 + (NumberFormattingHelpers.Uint16NumberBufferLength * 4) + 3 + 1];
        TryFormat(charBuffer, out var charsWritten, format, formatProvider);
        return charBuffer[..charsWritten].ToString();
    }

    [Pure]
    [DebuggerStepThrough]
    public override string ToString() => ToString(default, null);

    [SkipLocalsInit]
    public unsafe bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        int* p = stackalloc int[4];
        p[0] = Major;
        p[1] = Minor;
        p[2] = Build;
        p[3] = Revision;

        var components = Helpers.CreateReadOnlySpan<int>(p, 4);
        var formatParameters = new NumberFormattingHelpers.FormatParameters('[', '.', ']');
        return NumberFormattingHelpers.TryFormatIntegerSpan(components, destination, formatParameters, out charsWritten);
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(FileFormatVersion left, FileFormatVersion right)
    {
        var leftLong = left.GetBitsAsUlong();
        var rightLong = right.GetBitsAsUlong();

        return leftLong == rightLong;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(FileFormatVersion left, FileFormatVersion right) => !(left == right);

    [Pure]
    [DebuggerStepThrough]
    public static bool operator <(FileFormatVersion left, FileFormatVersion right)
    {
        var leftLong = left.GetBitsAsUlong();
        var rightLong = right.GetBitsAsUlong();

        return leftLong < rightLong;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator <=(FileFormatVersion left, FileFormatVersion right)
    {
        var leftLong = left.GetBitsAsUlong();
        var rightLong = right.GetBitsAsUlong();

        return leftLong <= rightLong;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator >(FileFormatVersion left, FileFormatVersion right)
    {
        var leftLong = left.GetBitsAsUlong();
        var rightLong = right.GetBitsAsUlong();

        return leftLong > rightLong;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator >=(FileFormatVersion left, FileFormatVersion right)
    {
        var leftLong = left.GetBitsAsUlong();
        var rightLong = right.GetBitsAsUlong();

        return leftLong >= rightLong;
    }
}
