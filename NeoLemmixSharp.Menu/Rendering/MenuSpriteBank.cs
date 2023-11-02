using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using NeoLemmixSharp.Menu.Widgets;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuSpriteBank
{
    private readonly Texture2D[] _textureLookup;

    public BackgroundBrush BackgroundBrush { get; }
    public SolidBrush TransparentBrush { get; }

    public MenuSpriteBank(ContentManager contentManager, GraphicsDevice graphicsDevice)
    {
        var numberOfResources = Enum.GetValuesAsUnderlyingType<MenuResource>().Length;

        _textureLookup = new Texture2D[numberOfResources];

        LoadMenuContent(contentManager, graphicsDevice);

        BackgroundBrush = new BackgroundBrush(GetTexture(MenuResource.Background));
        TransparentBrush = new SolidBrush(Color.Transparent);
    }

    private void LoadMenuContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
    {
        LoadResource(MenuResource.Background, "menu/background");
        LoadResource(MenuResource.Logo, "menu/logo");
        LoadResource(MenuResource.ScrollerLemmings, "menu/scroller_lemmings");
        LoadResource(MenuResource.ScrollerSegment, "menu/scroller_segment");
        LoadResource(MenuResource.SignPlay, "menu/sign_play");
        LoadResource(MenuResource.SignGroup, "menu/sign_group");
        LoadResource(MenuResource.SignGroupUp, "menu/sign_group_up");
        LoadResource(MenuResource.SignGroupDown, "menu/sign_group_down");
        LoadResource(MenuResource.SignLevelSelect, "menu/sign_level_select");
        LoadResource(MenuResource.SignConfig, "menu/sign_config");
        LoadResource(MenuResource.SignQuit, "menu/sign_quit");
        LoadResource(MenuResource.MenuButton, "menu/ui_button");

        var fadeTexture = new Texture2D(graphicsDevice, 1, 1);
        AddTexture(MenuResource.FadeTexture, fadeTexture);

        var cursorTexture = CreateCursorTexture_Debug(graphicsDevice);
        AddTexture(MenuResource.Cursor, cursorTexture);

        return;

        void LoadResource(MenuResource resource, string fileName)
        {
            var texture = contentManager.Load<Texture2D>(fileName);
            _textureLookup[(int)resource] = texture;
        }

        void AddTexture(MenuResource resource, Texture2D texture)
        {
            texture.Name = resource.ToString();
            _textureLookup[(int)resource] = texture;
        }
    }

    public Texture2D GetTexture(MenuResource resource) => _textureLookup[(int)resource];

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