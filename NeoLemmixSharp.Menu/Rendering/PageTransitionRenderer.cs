using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class PageTransitionRenderer : IDisposable
{
    private readonly PageTransition _pageTransition;

    private Size _windowSize;

    public bool IsTransitioning => _pageTransition.IsTransitioning;

    public PageTransitionRenderer(PageTransition pageTransition)
    {
        _pageTransition = pageTransition;
    }

    public void SetWindowDimensions(Size windowSize)
    {
        _windowSize = windowSize;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            CommonSprites.WhitePixelGradientSprite,
            Helpers.CreateRectangle(new Point(), _windowSize),
            CommonSprites.RectangleForWhitePixelAlpha(_pageTransition.TransitionAlpha),
            Color.Black);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
