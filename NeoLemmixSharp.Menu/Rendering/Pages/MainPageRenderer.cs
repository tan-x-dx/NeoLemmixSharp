using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace NeoLemmixSharp.Menu.Rendering.Pages;

public sealed class MainPageRenderer : IPageRenderer
{
    private readonly Texture2D _backgroundTexture;
    private readonly Texture2D _playButtonTexture;
    private readonly Texture2D _levelSelectButtonTexture;
    private readonly Texture2D _groupButtonTexture;
    private readonly Texture2D _groupUpButtonTexture;
    private readonly Texture2D _groupDownButtonTexture;
    private readonly Texture2D _configButtonTexture;
    private readonly Texture2D _quitButtonTexture;

    private int _windowWidth;
    private int _windowHeight;

    public MainPageRenderer(MenuSpriteBank menuSpriteBank)
    {
        _backgroundTexture = menuSpriteBank.GetTexture(MenuResource.Background);
        _playButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignPlay);
        _levelSelectButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignLevelSelect);
        _groupButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignGroup);
        _groupUpButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignGroupUp);
        _groupDownButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignGroupDown);
        _configButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignConfig);
        _quitButtonTexture = menuSpriteBank.GetTexture(MenuResource.SignQuit);
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        _windowWidth = windowWidth;
        _windowHeight = windowHeight;
    }

    public void SetRootWidget(Desktop desktop)
    {
        desktop.Root = null;
    }

    public void RenderPage(SpriteBatch spriteBatch)
    {
        PageRenderingHelpers.RenderBackground(_backgroundTexture, spriteBatch, _windowWidth, _windowHeight);
    }

    public void Dispose()
    {
    }
}