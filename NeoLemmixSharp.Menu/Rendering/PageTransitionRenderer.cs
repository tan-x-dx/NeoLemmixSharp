using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Rendering;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class PageTransitionRenderer : IDisposable
{
    private readonly PageTransition _pageTransition;
    private readonly Texture2D _fadeTexture;

    private int _windowWidth;
    private int _windowHeight;

    public PageTransitionRenderer(PageTransition pageTransition)
    {
        _pageTransition = pageTransition;
        _fadeTexture = CommonSprites.WhitePixelGradientSprite;
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        _windowWidth = windowWidth;
        _windowHeight = windowHeight;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        if (!_pageTransition.IsTransitioning)
            return;

        spriteBatch.Draw(
            _fadeTexture,
            new Rectangle(0, 0, _windowWidth, _windowHeight),
            CommonSprites.RectangleForWhitePixelAlpha(_pageTransition.TransitionAlpha),
            Color.Black);
    }

    public void Dispose()
    {
    }
}