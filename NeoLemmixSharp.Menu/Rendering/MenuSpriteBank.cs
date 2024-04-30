using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu.Rendering;

public static class MenuSpriteBank
{
    public static Texture2D Background { get; private set; } = null!;
    public static Texture2D Logo { get; private set; } = null!;
    public static Texture2D MenuIcons { get; private set; } = null!;
    public static Texture2D ScrollerLemmings { get; private set; } = null!;
    public static Texture2D ScrollerSegment { get; private set; } = null!;
    public static Texture2D SignPlay { get; private set; } = null!;
    public static Texture2D SignGroup { get; private set; } = null!;
    public static Texture2D SignGroupUp { get; private set; } = null!;
    public static Texture2D SignGroupDown { get; private set; } = null!;
    public static Texture2D SignLevelSelect { get; private set; } = null!;
    public static Texture2D SignConfig { get; private set; } = null!;
    public static Texture2D SignQuit { get; private set; } = null!;
    public static Texture2D MenuButton { get; private set; } = null!;
    public static Texture2D Cursor { get; private set; } = null!;

    public static void Initialise(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        Background = contentManager.Load<Texture2D>("menu/background");
        Logo = contentManager.Load<Texture2D>("menu/logo");
        MenuIcons = contentManager.Load<Texture2D>("menu/menu_icons");
        ScrollerLemmings = contentManager.Load<Texture2D>("menu/scroller_lemmings");
        ScrollerSegment = contentManager.Load<Texture2D>("menu/scroller_segment");
        SignPlay = contentManager.Load<Texture2D>("menu/sign_play");
        SignGroup = contentManager.Load<Texture2D>("menu/sign_group");
        SignGroupUp = contentManager.Load<Texture2D>("menu/sign_group_up");
        SignGroupDown = contentManager.Load<Texture2D>("menu/sign_group_down");
        SignLevelSelect = contentManager.Load<Texture2D>("menu/sign_level_select");
        SignConfig = contentManager.Load<Texture2D>("menu/sign_config");
        SignQuit = contentManager.Load<Texture2D>("menu/sign_quit");
        MenuButton = contentManager.Load<Texture2D>("menu/ui_button");

        Cursor = CreateCursorTexture_Debug(graphicsDevice);
    }

    private static Texture2D CreateCursorTexture_Debug(GraphicsDevice graphicsDevice)
    {
        var cursorTexture = new Texture2D(graphicsDevice, 32, 32);

        var white = Color.White.PackedValue;
        var red = new Color(200, 0, 0, 255).PackedValue;
        var blue = new Color(200, 0, 200, 255).PackedValue;

        var colors = new uint[32 * 32];
        for (var x = 0; x < 32; x++)
        {
            for (var y = 0; y < 32; y++)
            {
                if (x < y + y)
                {
                    colors[y * 32 + x] = white;
                }

                if (x == y + y)
                {
                    colors[y * 32 + x] = blue;
                }

                if (x + x == y)
                {
                    colors[y * 32 + x] = red;
                }

                if (x + x < y)
                {
                    colors[y * 32 + x] = 0U;
                }
            }
        }

        for (var x = 16; x < 32; x++)
        {
            for (var y = 16; y < 32; y++)
            {
                colors[y * 32 + x] = 0U;
            }
        }

        cursorTexture.SetData(colors);

        return cursorTexture;
    }
}