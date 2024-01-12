using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level;

[Flags]
public enum LevelParameters
{
	TimedBombers = 1 << 0,
	EnablePause = 1 << 1,
	EnableNuke = 1 << 2,
	EnableFastForward = 1 << 3,
	EnableDirectionSelect = 1 << 4,
	EnableClearPhysics = 1 << 5,
	EnableSkillShadows = 1 << 6,
	EnableFrameControl = 1 << 7,
}

public static class LevelParameterHelpers
{
	public static bool TestFlag(this LevelParameters parameters, LevelParameters test)
	{
		return (parameters & test) != 0;
	}

	public static int GetLemmingCountDownTimer(this LevelParameters parameters, Lemming lemming)
	{
		var timedBombers = parameters.TestFlag(LevelParameters.TimedBombers);

		if (timedBombers)
			return lemming.IsFastForward
				? LevelConstants.DefaultFastForwardLemmingCountDownActionTicks
				: LevelConstants.DefaultCountDownActionTicks;

		return 1; // I.e. the next frame
	}
}