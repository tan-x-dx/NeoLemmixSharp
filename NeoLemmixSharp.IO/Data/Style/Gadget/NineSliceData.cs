using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct NineSliceData(int nineSliceDown, int nineSliceLeft, int nineSliceUp, int nineSliceRight) : IEquatable<NineSliceData>
{
    public readonly int NineSliceDown = nineSliceDown;
    public readonly int NineSliceLeft = nineSliceLeft;
    public readonly int NineSliceUp = nineSliceUp;
    public readonly int NineSliceRight = nineSliceRight;

    public bool Equals(NineSliceData other) => NineSliceDown == other.NineSliceDown &&
                                               NineSliceLeft == other.NineSliceLeft &&
                                               NineSliceUp == other.NineSliceUp &&
                                               NineSliceRight == other.NineSliceRight;

    public override bool Equals([NotNullWhen(true)] object? obj)=> obj is NineSliceData other && Equals(other);
    public override int GetHashCode() => 6320333 * NineSliceDown +
                                         7287817 * NineSliceLeft +
                                         2059537 * NineSliceUp +
                                         4887481 * NineSliceRight +
                                         5119139;
    public static bool operator ==(NineSliceData left, NineSliceData right) => left.Equals(right);
    public static bool operator !=(NineSliceData left, NineSliceData right) => !left.Equals(right);
}
