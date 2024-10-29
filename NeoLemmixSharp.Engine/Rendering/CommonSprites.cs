﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using System.Runtime.CompilerServices;

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
        var anchorTexture = new Texture2D(graphicsDevice, 4, 4 * 4)
        {
            Name = AnchorName
        };

        var red = new Color((byte)200, (byte)0, (byte)0, (byte)255);
        var yellow = new Color((byte)200, (byte)200, (byte)0, (byte)255);
        var purple = new Color((byte)200, (byte)0, (byte)200, (byte)255);

        var x = new Color[4 * 4 * 4];

        x[1] = red;
        x[4] = red;
        x[5] = purple;
        x[6] = red;
        x[8] = red;
        x[9] = yellow;
        x[10] = red;
        x[13] = red;

        x[17] = red;
        x[18] = red;
        x[20] = red;
        x[21] = yellow;
        x[22] = purple;
        x[23] = red;
        x[25] = red;
        x[26] = red;

        x[34] = red;
        x[37] = red;
        x[38] = yellow;
        x[39] = red;
        x[41] = red;
        x[42] = purple;
        x[43] = red;
        x[46] = red;

        x[53] = red;
        x[54] = red;
        x[56] = red;
        x[57] = purple;
        x[58] = yellow;
        x[59] = red;
        x[61] = red;
        x[62] = red;

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
            whiteColors[i] = new Color((byte)0xff, (byte)0xff, (byte)0xff, (byte)i);
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
            CursorSprite,
            MenuSpriteBank.CursorHiRes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle RectangleForWhitePixelAlpha(int alpha)
    {
        return new Rectangle(0, alpha & 0xff, 1, 1);
    }
}