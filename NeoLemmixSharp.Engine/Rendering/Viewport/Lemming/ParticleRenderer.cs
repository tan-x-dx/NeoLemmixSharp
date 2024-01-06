using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public static class ParticleRenderer
{
	private static readonly byte[] ParticleOffsets =
		new byte[2 * LevelConstants.ParticleFrameCount * LevelConstants.NumberOfParticles];

	public static Span<byte> GetParticleOffsetBuffer() => new(ParticleOffsets);

	public static LevelPosition GetParticleOffsets(
		int frame,
		int particleId)
	{
		frame = LevelConstants.ParticleFrameCount - frame;

		var index = 2 * (LevelConstants.NumberOfParticles * frame + particleId);

		var s = new Span<byte>(ParticleOffsets, index, 2);

		return new LevelPosition(s[0] - 128, s[1] - 128);
	}
}