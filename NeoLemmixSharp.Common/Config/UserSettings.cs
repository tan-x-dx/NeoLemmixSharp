using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics;

namespace NeoLemmixSharp.Common.Config;

public sealed class UserSettings
{
    public string UserName { get; set; } = null!;
    public GameplaySettings GameplaySettings { get; } = new();

    public void SetUpLevelParameters(BitArraySet<LevelParameterHasher, BitBuffer32, LevelParameters> levelParameters)
    {
        Debug.Assert(levelParameters.Count == 0);

        GameplaySettings.SetUpLevelParameters(levelParameters);
    }

    public void SetUpControlPanelParameters(BitArraySet<ControlPanelParameterHasher, BitBuffer32, ControlPanelParameters> controlPanelParameters)
    {
        Debug.Assert(controlPanelParameters.Count == 0);

        GameplaySettings.SetUpControlPanelParameters(controlPanelParameters);
    }
}
