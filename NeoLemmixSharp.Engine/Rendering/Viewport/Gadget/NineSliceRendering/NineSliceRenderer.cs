using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget.NineSliceRendering;

public sealed class NineSliceRenderer : IViewportObjectRenderer
{
    private readonly IRectangularLevelRegion _rectangularLevelRegion;
    private readonly NineSliceSubRenderer[] _subRenderers;

    private readonly Texture2D _texture;

    public NineSliceRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int sliceTop,
        int sliceBottom,
        int sliceLeft,
        int sliceRight)
    {
        _rectangularLevelRegion = rectangularLevelRegion;

        _subRenderers = GetSubRenderers(
            rectangularLevelRegion,
            spriteWidth,
            spriteHeight,
            sliceTop,
            sliceBottom,
            sliceLeft,
            sliceRight);

        _texture = texture;
    }

    private static NineSliceSubRenderer[] GetSubRenderers(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceTop,
        int sliceBottom,
        int sliceLeft,
        int sliceRight)
    {
        var results = new NineSliceSubRenderer[]
        {
            new NineSliceTopLeftCornerRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceLeft, sliceTop),
            //new NineSliceTopRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceLeft, sliceRight, sliceTop),
            new NineSliceTopRightCornerRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceRight, sliceTop),

            //new NineSliceLeftRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceTop, sliceBottom, sliceLeft),
            //new NineSliceCentreRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceLeft, sliceRight, sliceTop, sliceBottom),
            //new NineSliceRightRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceTop, sliceBottom, sliceRight),

            //new NineSliceBottomLeftCornerRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceLeft, sliceBottom),
            //new NineSliceBottomRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceLeft, sliceRight, sliceBottom),
            //new NineSliceBottomRightCornerRenderer(rectangularLevelRegion, spriteWidth, spriteHeight, sliceRight, sliceBottom)
        };

        return results
            .Where(r => !r.IsEmpty)
            .ToArray();
    }

    public Rectangle GetSpriteBounds() => _rectangularLevelRegion.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        for (var i = 0; i < _subRenderers.Length; i++)
        {
            _subRenderers[i].Render(spriteBatch, _texture, sourceRectangle, screenX, screenY, scaleMultiplier);
        }
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}