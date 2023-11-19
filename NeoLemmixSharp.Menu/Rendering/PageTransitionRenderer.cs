using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class PageTransitionRenderer : IDisposable
{
    private readonly PageTransition _pageTransition;
    private readonly uint[] _pixelSet;
    private readonly Texture2D _fadeTexture;

    private int _windowWidth;
    private int _windowHeight;

    public PageTransitionRenderer(PageTransition pageTransition)
    {
        _pageTransition = pageTransition;
        _fadeTexture = MenuSpriteBank.GetTexture(MenuResource.FadeTexture);
        _pixelSet = new uint[1];
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

        _pixelSet[0] = _pageTransition.TransitionPackedColor;

        _fadeTexture.SetData(_pixelSet);
        spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, _windowWidth, _windowHeight), Color.White);
    }

    private static uint GetPackedFadeColor(double alpha)
    {
        var intValue = (uint)(alpha * 255d);
        intValue = Math.Clamp(intValue, 0u, 0xffu);

        // Color format is ABGR - alpha is the most significant bits and everything else is black (zero)
        return intValue << 24;
    }

    public void Dispose()
    {
        var blackFadeColor = Color.Transparent.PackedValue;
        _pixelSet[0] = blackFadeColor;

        _fadeTexture.SetData(_pixelSet);
    }
}