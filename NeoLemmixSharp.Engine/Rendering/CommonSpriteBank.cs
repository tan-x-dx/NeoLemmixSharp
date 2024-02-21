using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class CommonSpriteBank
{
    public static CommonSpriteBank Instance { get; private set; } = null!;

    public static void Initialise(Texture2D[] textureLookup)
    {
        Instance = new CommonSpriteBank(textureLookup);
    }

    private readonly Texture2D[] _textureLookup;

    private CommonSpriteBank(Texture2D[] textureLookup)
    {
        _textureLookup = textureLookup;
    }

    public Texture2D GetTexture(CommonTexture textureName)
    {
        return _textureLookup[(int)textureName];
    }

    public LevelCursorSprite GetLevelCursorSprite(LevelCursor levelCursor)
    {
        return new LevelCursorSprite(
            levelCursor,
            GetTexture(CommonTexture.LevelCursors));
    }
}