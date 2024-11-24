using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu;

public static class MenuSpriteBank
{
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

    public static void Initialise(ContentManager contentManager)
    {
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
    }
}