using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
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
    public static Span<byte> GetByteBuffer() => MemoryMarshal.Cast<sbyte, byte>(new Span<sbyte>(ParticleOffsets));

    [Pure]
    public static LevelPosition GetParticleOffsets(
        int frame,
        int particleId)
    {
        var index = GetParticleIndex(frame, particleId);

        var span = new ReadOnlySpan<sbyte>(ParticleOffsets, index, NumberOfBytesPerCall);

        return new LevelPosition(span[0], span[1]);
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