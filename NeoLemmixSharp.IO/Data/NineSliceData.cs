using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data;

public readonly struct NineSliceData(int nineSliceBottom, int nineSliceLeft, int nineSliceTop, int nineSliceRight) : IEquatable<NineSliceData>
{
    public readonly int NineSliceBottom = nineSliceBottom;
    public readonly int NineSliceLeft = nineSliceLeft;
    public readonly int NineSliceTop = nineSliceTop;
    public readonly int NineSliceRight = nineSliceRight;

    public bool Equals(NineSliceData other) => NineSliceBottom == other.NineSliceBottom &&
                                               NineSliceLeft == other.NineSliceLeft &&
                                               NineSliceTop == other.NineSliceTop &&
                                               NineSliceRight == other.NineSliceRight;

    public override bool Equals([NotNullWhen(true)] object? obj)=> obj is NineSliceData other && Equals(other);
    public override int GetHashCode() => 6320333 * NineSliceBottom +
                                         7287817 * NineSliceLeft +
                                         2059537 * NineSliceTop +
                                         4887481 * NineSliceRight +
                                         5119139;
    public static bool operator ==(NineSliceData left, NineSliceData right) => left.Equals(right);
    public static bool operator !=(NineSliceData left, NineSliceData right) => !left.Equals(right);
}
