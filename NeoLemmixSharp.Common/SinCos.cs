using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public static class SinCosMethods
{
    /// <summary>
    /// Computes the Sine and Cosine of an angle, interpreting the input as an integer multiple of pi/2 radians.
    /// This method maps <see langword="int" />s to <see langword="int" />s, and avoids any floating point calculations.
    /// </summary>
    /// <param name="theta">The angle as a multiple of pi/2 radians.</param>
    /// <returns>The Sine and Cosine of that angle, as a pair of <see langword="int" />s.</returns>
    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SinCos IntSinCos(int theta)
    {
        ReadOnlySpan<int> RawInts =
        [
             0,  1,
             1,  0,
             0, -1,
            -1,  0
        ];

        var sinCosData = MemoryMarshal.Cast<int, SinCos>(RawInts);

        return sinCosData[theta & 3];
    }
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct SinCos
{
    [FieldOffset(0 * sizeof(int))] public readonly int Sin;
    [FieldOffset(1 * sizeof(int))] public readonly int Cos;
}
