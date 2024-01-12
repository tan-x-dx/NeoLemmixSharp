using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;

public sealed class NineSliceRenderer : IViewportObjectRenderer
{
    private readonly IRectangularLevelRegion _spriteClipRectangle;
    private readonly NineSliceSubRenderer[] _subRenderers;

    private readonly Texture2D _texture;

    public NineSliceRenderer(
        IRectangularLevelRegion spriteClipRectangle,
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int sliceTop,
        int sliceBottom,
        int sliceLeft,
        int sliceRight)
    {
        _spriteClipRectangle = spriteClipRectangle;

        _subRenderers = GetSubRenderers(
            spriteClipRectangle,
            spriteWidth,
            spriteHeight,
            sliceTop,
            sliceBottom,
            sliceLeft,
            sliceRight);

        _texture = texture;
    }

    private static NineSliceSubRenderer[] GetSubRenderers(
        IRectangularLevelRegion spriteClipRectangle,
        int spriteWidth,
        int spriteHeight,
        int sliceTop,
        int sliceBottom,
        int sliceLeft,
        int sliceRight)
    {
        var results = new NineSliceSubRenderer[]
        {
            new NineSliceTopLeftCornerRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceLeft, sliceTop),
            new NineSliceTopRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceLeft, sliceRight, sliceTop),
            new NineSliceTopRightCornerRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceRight, sliceTop),

            //new NineSliceLeftRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceTop, sliceBottom, sliceLeft),
            //new NineSliceCentreRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceLeft, sliceRight, sliceTop, sliceBottom),
            //new NineSliceRightRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceTop, sliceBottom, sliceRight),

            new NineSliceBottomLeftCornerRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceLeft, sliceBottom),
            new NineSliceBottomRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceLeft, sliceRight, sliceBottom),
            new NineSliceBottomRightCornerRenderer(spriteClipRectangle, spriteWidth, spriteHeight, sliceRight, sliceBottom)
        };

        return results
            .Where(r => !r.IsEmpty)
            .ToArray();
    }

    public Rectangle GetSpriteBounds() => _spriteClipRectangle.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        for (var i = 0; i < _subRenderers.Length; i++)
        {
            _subRenderers[i].Render(spriteBatch, _texture, sourceRectangle, screenX, screenY, scaleMultiplier);
        }
    }

    public void Dispose()
    {
    }
}