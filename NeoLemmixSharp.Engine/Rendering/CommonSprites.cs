using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Rendering;

public static class CommonSprites
{
    private const string AnchorName = "LemmingAnchorTexture";
    private const string WhitePixelName = "WhitePixel";

    public static Texture2D AnchorSprite { get; private set; } = null!;
    public static Texture2D WhitePixelGradientSprite { get; private set; } = null!;
    public static Texture2D CursorSprite { get; private set; } = null!;

    public static void Initialise(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        AnchorSprite = CreateAnchorTexture(graphicsDevice);
        WhitePixelGradientSprite = CreateWhitePixelTexture(graphicsDevice);
        CursorSprite = LoadCursorSprites(contentManager);
    }

    private static Texture2D CreateAnchorTexture(GraphicsDevice graphicsDevice)
    {
        var anchorTexture = new Texture2D(graphicsDevice, 3, 3)
        {
            Name = AnchorName
        };

        var red = new Color(200, 0, 0, 255).PackedValue;
        var yellow = new Color(200, 200, 0, 255).PackedValue;

        var x = new uint[9];
        x[1] = red;
        x[3] = red;
        x[4] = yellow;
        x[5] = red;
        x[7] = red;
        anchorTexture.SetData(x);

        return anchorTexture;
    }

    private static Texture2D CreateWhitePixelTexture(GraphicsDevice graphicsDevice)
    {
        var whitePixelTexture = new Texture2D(graphicsDevice, 1, 256)
        {
            Name = WhitePixelName
        };

        var whiteColors = new Color[256];
        for (var i = 0; i < whiteColors.Length; i++)
        {
            whiteColors[i] = new Color(0xff, 0xff, 0xff, i);
        }

        whitePixelTexture.SetData(whiteColors);
        return whitePixelTexture;
    }

    private static Texture2D LoadCursorSprites(ContentManager contentManager)
    {
        return contentManager.Load<Texture2D>("cursor/cursors");
    }

    public static LevelCursorSprite GetLevelCursorSprite(LevelCursor levelCursor)
    {
        return new LevelCursorSprite(
            levelCursor,
            CursorSprite);
    }
}