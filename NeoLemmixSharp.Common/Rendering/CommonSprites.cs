using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Rendering;

public static class CommonSprites
{
    private const string AnchorName = "LemmingAnchorTexture";
    private const string WhitePixelName = "WhitePixel";

    public const int CursorHiResXOffset = -4;
    public const int CursorHiResYOffset = -4;
    public const int CursorHiResWidth = 25;
    public const int CursorHiResHeight = 28;

    public static Texture2D AnchorSprite { get; private set; } = null!;
    public static Texture2D WhitePixelGradientSprite { get; private set; } = null!;

    public static Texture2D Background { get; private set; } = null!;
    public static Texture2D CursorHand { get; private set; } = null!;
    public static Texture2D CursorLoading { get; private set; } = null!;
    public static Texture2D CursorHandHiRes { get; private set; } = null!;
    public static Texture2D CursorLoadingHiRes { get; private set; } = null!;

    public static Texture2D CursorCrossHair { get; private set; } = null!;

    public static void Initialise(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        if (AnchorSprite is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(CommonSprites)} more than once!");

        AnchorSprite = CreateAnchorTexture(graphicsDevice);
        WhitePixelGradientSprite = CreateWhitePixelTexture(graphicsDevice);

        Background = contentManager.Load<Texture2D>("menu/background");
        CursorHand = contentManager.Load<Texture2D>("cursor/amiga");
        CursorLoading = contentManager.Load<Texture2D>("cursor/loading");
        CursorHandHiRes = contentManager.Load<Texture2D>("cursor/amiga_hr");
        CursorLoadingHiRes = contentManager.Load<Texture2D>("cursor/loading_hr");

        CursorCrossHair = contentManager.Load<Texture2D>("cursor/cursors");
    }

    private static Texture2D CreateAnchorTexture(GraphicsDevice graphicsDevice)
    {
        var anchorTexture = new Texture2D(graphicsDevice, 4, 4 * 4)
        {
            Name = AnchorName
        };

        var red = new Color(0xff0000c8);
        var yellow = new Color(0xff00c8c8);
        var purple = new Color(0xffc800c8);

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
            var packedValue = (uint)(i << 24 | 0xffffff);
            whiteColors[i] = new Color(packedValue);
        }

        whitePixelTexture.SetData(whiteColors);
        return whitePixelTexture;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle RectangleForWhitePixelAlpha(int alpha)
    {
        return new Rectangle(0, alpha & 0xff, 1, 1);
    }
}