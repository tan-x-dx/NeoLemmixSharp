using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

    public static void Initialise(ContentManager contentManager)
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
    }
}