using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

internal sealed class ThemeData
{
    public string SpriteFolder { get; set; }

    private readonly LemmingActionSpriteData[] _lemmingActionSpriteData = new LemmingActionSpriteData[EngineConstants.MaxNumberOfTribes * EngineConstants.NumberOfLemmingActions];

    public LemmingActionSpriteData GetLemmingActionSpriteData(int tribeId, int lemmingActionId)
    {
        AssertIdsAreValid(tribeId, lemmingActionId);

        var index = (tribeId * EngineConstants.NumberOfLemmingActions) + lemmingActionId;
        return _lemmingActionSpriteData[index];
    }

    private static void AssertIdsAreValid(int tribeId, int lemmingActionId)
    {
        if ((uint)tribeId >= EngineConstants.MaxNumberOfTribes)
            throw new InvalidOperationException($"Invalid tribe ID: {tribeId}");

        if (!EngineConstants.IsValidLemmingActionId(lemmingActionId))
            throw new InvalidOperationException($"Invalid lemming action ID: {lemmingActionId}");
    }
}
