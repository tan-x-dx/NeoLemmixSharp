using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Font;
using MLEM.Textures;
using MLEM.Ui.Style;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Screen;

public static class MenuSpriteBank
{
    public const int CursorHiResXOffset = -4;
    public const int CursorHiResYOffset = -4;
    public const int CursorHiResWidth = 25;
    public const int CursorHiResHeight = 28;
    
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
    public static Texture2D CursorLoading { get; private set; } = null!;
    public static Texture2D CursorHiRes { get; private set; } = null!;
    public static Texture2D CursorLoadingHiRes { get; private set; } = null!;

    public static UntexturedStyle MenuStyle { get; private set; } = null!;

    public static void Initialise(ContentManager contentManager, SpriteBatch spriteBatch)
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
        Cursor = contentManager.Load<Texture2D>("cursor/amiga");
        CursorLoading = contentManager.Load<Texture2D>("cursor/loading");
        CursorHiRes = contentManager.Load<Texture2D>("cursor/amiga_hr");
        CursorLoadingHiRes = contentManager.Load<Texture2D>("cursor/loading_hr");

        var testTexture = contentManager.Load<Texture2D>("menu/Test");
        var testPatch = new NinePatch(new TextureRegion(testTexture, 0, 8, 24, 24), 8);
        MenuStyle = new UntexturedStyle(spriteBatch)
        {
            // when using a SpriteFont, use GenericSpriteFont. When using a MonoGame.Extended BitmapFont, use GenericBitmapFont.
            // Wrapping fonts like this allows for both types to be usable within MLEM.Ui easily
            // Supplying a bold and an italic version is optional
            Font = new GenericSpriteFont(
                    contentManager.Load<SpriteFont>("Fonts/TestFont"),
                    contentManager.Load<SpriteFont>("Fonts/TestFontBold"),
                    contentManager.Load<SpriteFont>("Fonts/TestFontItalic")),
            TextScale = 0.1F,
            PanelTexture = testPatch,
            ButtonTexture = new NinePatch(new TextureRegion(testTexture, 24, 8, 16, 16), 4),
            TextFieldTexture = new NinePatch(new TextureRegion(testTexture, 24, 8, 16, 16), 4),
            ScrollBarBackground = new NinePatch(new TextureRegion(testTexture, 12, 0, 4, 8), 1, 1, 2, 2),
            ScrollBarScrollerTexture = new NinePatch(new TextureRegion(testTexture, 8, 0, 4, 8), 1, 1, 2, 2),
            CheckboxTexture = new NinePatch(new TextureRegion(testTexture, 24, 8, 16, 16), 4),
            CheckboxCheckmark = new TextureRegion(testTexture, 24, 0, 8, 8),
            RadioTexture = new NinePatch(new TextureRegion(testTexture, 16, 0, 8, 8), 3),
            RadioCheckmark = new TextureRegion(testTexture, 32, 0, 8, 8)
        };
    }
}