using NeoLemmixSharp.Io.LevelReading.Data.SpriteSet;

namespace NeoLemmixSharp.Io.LevelReading.Data;

public sealed class ThemeData
{
    public string BaseStyle { get; set; }
    public string LemmingSpritesFilePath { get; set; }

    public uint Mask { get; set; }
    public uint Minimap { get; set; }
    public uint Background { get; set; }
    public uint OneWays { get; set; }
    public uint PickupBorder { get; set; }
    public uint PickupInside { get; set; }

    public LemmingSpriteSetRecolouring LemmingSpriteSetRecolouring { get; } = new();
    public Dictionary<string, LemmingStateRecolouring> LemmingStateRecolouringLookup { get; } = new();
    public Dictionary<string, LemmingSpriteData> LemmingSpriteDataLookup { get; } = new();
}