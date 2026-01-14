using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Updates;

public enum UpdateState
{
    Paused = 0,
    Normal = 1,
    FastForward = EngineConstants.FastForwardSpeedMultiplier
}
