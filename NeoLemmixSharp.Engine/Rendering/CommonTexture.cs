namespace NeoLemmixSharp.Engine.Rendering;

public enum CommonTexture
{
    WhitePixel,
    LemmingAnchorTexture,
    LevelCursors,
}

public static class CommonTexturesStringHelper
{
    public static string GetTexturePath(this CommonTexture texture) => texture switch
    {
        CommonTexture.WhitePixel => "WhitePixel",
        CommonTexture.LemmingAnchorTexture => "LemmingAnchorTexture",
        CommonTexture.LevelCursors => "cursor/cursors",

        _ => throw new ArgumentOutOfRangeException(nameof(texture), texture, "Unknown texture")
    };
}