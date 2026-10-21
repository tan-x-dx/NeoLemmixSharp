using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public static class ParticleHelper
{
    private const int NumberOfBytesPerCall = 2;
    private const int ByteLength = NumberOfBytesPerCall * EngineConstants.NumberOfParticles * EngineConstants.ParticleFrameCount;

    private static readonly Point[] ParticleOffsets = new Point[EngineConstants.NumberOfParticles * EngineConstants.ParticleFrameCount];

    [Pure]
    public static Span<byte> GetByteBuffer() => new(new byte[ByteLength]);

    public static void SetByteData(ReadOnlySpan<byte> rawBytes)
    {
        Debug.Assert(rawBytes.Length == ByteLength);

        var i = 0;
        var j = 0;
        do
        {
            var x = (int)(sbyte)rawBytes.At(i++);
            var y = (int)(sbyte)rawBytes.At(i++);

            ParticleOffsets.At(j++) = new Point(x, y);
        }
        while (i < rawBytes.Length);

        Debug.Assert(i == rawBytes.Length);
        Debug.Assert(j == ParticleOffsets.Length);
    }

    [Pure]
    public static Point GetParticleOffsets(
        int frame,
        int particleId)
    {
        var index = GetParticleIndex(frame, particleId);
        return ParticleOffsets[index];
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetParticleIndex(
        int frame,
        int particleId)
    {
        return EngineConstants.NumberOfParticles * (EngineConstants.ParticleFrameCount - frame) + particleId;
    }
}
