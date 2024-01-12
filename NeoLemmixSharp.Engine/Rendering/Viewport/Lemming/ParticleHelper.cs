using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public static class ParticleHelper
{
	private const int NumberOfBytesPerCall = 2;

	public const int ByteLength = NumberOfBytesPerCall * LevelConstants.ParticleFrameCount * LevelConstants.NumberOfParticles;

	private static readonly sbyte[] ParticleOffsets = new sbyte[ByteLength];

	public static void InitialiseParticleOffsets(Span<byte> byteBuffer)
	{
		var sbyteBuffer = MemoryMarshal.Cast<byte, sbyte>(byteBuffer);
		var destSpan = new Span<sbyte>(ParticleOffsets);
		sbyteBuffer.CopyTo(destSpan);
	}

	[Pure]
	public static LevelPosition GetParticleOffsets(
		int frame,
		int particleId)
	{
		var index = GetParticleIndex(frame, particleId);

		var span = new Span<sbyte>(ParticleOffsets, index, NumberOfBytesPerCall);

		return new LevelPosition(span[0], span[1]);
	}

	[Pure]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetParticleIndex(
		int frame,
		int particleId)
	{
		frame = LevelConstants.ParticleFrameCount - frame;

		return NumberOfBytesPerCall * (LevelConstants.NumberOfParticles * frame + particleId);
	}
}