using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public static class ParticleHelper
{
	private const int NumberOfBytesPerCall = 2;

	public const int ByteLength = NumberOfBytesPerCall * LevelConstants.ParticleFrameCount * LevelConstants.NumberOfParticles;

	private static readonly sbyte[] ParticleOffsets = new sbyte[ByteLength];

	public static void InitialiseParticleOffsets(Span<byte> byteBuffer)
	{
		var destSpan = new Span<sbyte>(ParticleOffsets);
		if (byteBuffer.Length != destSpan.Length)
			throw new InvalidOperationException("Buffer length mismatch");

		for (var i = 0; i < destSpan.Length; i++)
		{
			destSpan[i] = (sbyte)byteBuffer[i];
		}
	}

	[Pure]
	public static LevelPosition GetParticleOffsets(
		int frame,
		int particleId)
	{
		var index = GetParticleIndex(frame, particleId);

		var s = new Span<sbyte>(ParticleOffsets, index, NumberOfBytesPerCall);

		return new LevelPosition(s[0], s[1]);
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