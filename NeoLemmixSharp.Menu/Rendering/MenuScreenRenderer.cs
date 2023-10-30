using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly FontBank _fontBank;

    private readonly CursorRenderer _cursorRenderer;

    private Texture2D _backGround;

    public bool IsDisposed { get; private set; }
    public IGameWindow GameWindow { get; set; }

    private int _backGroundTileX;
    private int _backGroundTileY;

    public MenuScreenRenderer(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank,
        CursorRenderer cursorRenderer)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _fontBank = fontBank;
        _cursorRenderer = cursorRenderer;
    }

    public void Initialise()
    {
        _backGround = _contentManager.Load<Texture2D>("background");


        RecalculateBackgroundTileValues();
    }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        RenderBackground(spriteBatch);

        _cursorRenderer.RenderCursor(spriteBatch);
    }

    private void RenderBackground(SpriteBatch spriteBatch)
    {
        var spriteWidth = _backGround.Width;
        var spriteHeight = _backGround.Height;

        for (var x = 0; x < _backGroundTileX; x++)
        {
            var vx = x * spriteWidth;

            for (var y = 0; y < _backGroundTileY; y++)
            {
                var vy = y * spriteHeight;

                spriteBatch.Draw(_backGround, new Vector2(vx, vy), Color.White);
            }
        }
    }

    public void OnWindowSizeChanged()
    {
        RecalculateBackgroundTileValues();
    }

    private void RecalculateBackgroundTileValues()
    {
        var spriteWidth = _backGround.Width;
        var spriteHeight = _backGround.Height;

        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        _backGroundTileX = 1 + windowWidth / spriteWidth;
        _backGroundTileY = 1 + windowHeight / spriteHeight;
    }

    public void Dispose()
    {
        HelperMethods.DisposeOf(ref _backGround);
        _cursorRenderer.Dispose();
    }
}