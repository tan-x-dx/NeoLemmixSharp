using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class PageTransitionRenderer : IDisposable
{
    private readonly PageTransition _pageTransition;
    private readonly Texture2D _fadeTexture;

    private LevelSize _windowSize;

    public PageTransitionRenderer(PageTransition pageTransition)
    {
        _pageTransition = pageTransition;
        _fadeTexture = CommonSprites.WhitePixelGradientSprite;
    }

    public void SetWindowDimensions(LevelSize windowSize)
    {
        _windowSize = windowSize;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        if (!_pageTransition.IsTransitioning)
            return;

        spriteBatch.Draw(
            _fadeTexture,
            Helpers.CreateRectangle(new LevelPosition(), _windowSize),
            CommonSprites.RectangleForWhitePixelAlpha(_pageTransition.TransitionAlpha),
            Color.Black);
    }

    public void Dispose()
    {
    }
}