using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public static class ParticleHelper
{
    private const int NumberOfBytesPerCall = 2;
    private const int ByteLength = NumberOfBytesPerCall * EngineConstants.NumberOfParticles * EngineConstants.ParticleFrameCount;

    private static readonly sbyte[] ParticleOffsets = new sbyte[ByteLength];

    [Pure]
    public static Span<byte> GetByteBuffer() => MemoryMarshal.Cast<sbyte, byte>(ParticleOffsets);

    [Pure]
    public static Point GetParticleOffsets(
        int frame,
        int particleId)
    {
        var index = GetParticleIndex(frame, particleId);
        return new Point(ParticleOffsets[index], ParticleOffsets[index + 1]);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetParticleIndex(
        int frame,
        int particleId)
    {
        return NumberOfBytesPerCall *
               (EngineConstants.NumberOfParticles * (EngineConstants.ParticleFrameCount - frame) + particleId);
    }
}