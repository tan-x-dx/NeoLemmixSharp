using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

internal sealed class ThemeData
{
    public string SpriteFolder { get; set; }

    private readonly LemmingActionSpriteData[] _lemmingActionSpriteData = new LemmingActionSpriteData[EngineConstants.NumberOfLemmingActions];
    private readonly TribeColorData[] _tribeColorData = new TribeColorData[EngineConstants.MaxNumberOfTribes];

    public Color Mask { get; set; }
    public Color Minimap { get; set; }
    public Color Background { get; set; }
    public Color OneWayArrows { get; set; }
    public Color PickupBorder { get; set; }
    public Color PickupInside { get; set; }

    public LemmingActionSpriteData GetLemmingActionSpriteData(int lemmingActionId)
    {
        AssertActionIdIsValid(lemmingActionId);

        return _lemmingActionSpriteData[lemmingActionId];
    }

    private static void AssertActionIdIsValid(int lemmingActionId)
    {
        if (!EngineConstants.IsValidLemmingActionId(lemmingActionId))
            throw new InvalidOperationException($"Invalid lemming action ID: {lemmingActionId}");
    }
}
