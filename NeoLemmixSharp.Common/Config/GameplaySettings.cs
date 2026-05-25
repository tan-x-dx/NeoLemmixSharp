using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Config;

public sealed class GameplaySettings
{
    public bool UseTimedBombers { get; set; }
    public bool EnableSkillShadows { get; set; }
    public bool UseSpawnInterval { get; set; }

    internal void SetUpLevelParameters(BitArraySet<LevelParameterHasher, BitBuffer32, LevelParameters> levelParameters)
    {
        if (UseTimedBombers) levelParameters.Add(LevelParameters.TimedBombers);
        if (EnableSkillShadows) levelParameters.Add(LevelParameters.EnableSkillShadows);
    }

    internal void SetUpControlPanelParameters(BitArraySet<ControlPanelParameterHasher, BitBuffer32, ControlPanelParameters> controlPanelParameters)
    {
        if (UseSpawnInterval) controlPanelParameters.Add(ControlPanelParameters.ShowSpawnIntervalInsteadOfReleaseRate);
    }
}
