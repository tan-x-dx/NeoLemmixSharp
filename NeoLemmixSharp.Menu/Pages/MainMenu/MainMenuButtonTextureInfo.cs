using NeoLemmixSharp.Menu.Components;

namespace NeoLemmixSharp.Menu.Pages.MainMenu;

public static class MainMenuButtonTextureInfo
{
    public static TextureInfo PlayButtonTextureInfo => new()
    {
        TextureName = "menu/sign_play.png",

        TextureWidth = 120f,
        TextureHeight = 87f
    };

    public static TextureInfo LevelSelectButtonTextureInfo => new()
    {
        TextureName = "menu/sign_level_select.png",

        TextureWidth = 120f,
        TextureHeight = 87f
    };

    public static TextureInfo ConfigButtonTextureInfo => new()
    {
        TextureName = "menu/sign_config.png",

        TextureWidth = 120f,
        TextureHeight = 87f
    };

    public static TextureInfo QuitButtonTextureInfo => new()
    {
        TextureName = "menu/sign_quit.png",

        TextureWidth = 120f,
        TextureHeight = 87f
    };
}
