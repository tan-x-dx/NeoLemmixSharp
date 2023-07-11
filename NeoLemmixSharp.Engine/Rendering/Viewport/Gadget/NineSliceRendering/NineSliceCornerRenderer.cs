using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget.NineSliceRendering;

public sealed class NineSliceTopLeftCornerRenderer : NineSliceSubRenderer
{
    private readonly int _sliceLeft;
    private readonly int _sliceTop;

    public override bool IsEmpty => _sliceLeft == 0 || _sliceTop == 0;

    public NineSliceTopLeftCornerRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceLeft,
        int sliceTop)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceLeft = sliceLeft;
        _sliceTop = sliceTop;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        var subRectangle = new Rectangle(0, 0, _sliceLeft, _sliceTop);
        Rectangle.Intersect(ref sourceRectangle, ref subRectangle, out var intersection);
        if (intersection.IsEmpty)
            return;

        var destinationRectangle = new Rectangle(
            screenX - (intersection.X * scaleMultiplier),
            screenY - (intersection.Y * scaleMultiplier),
            intersection.Width * scaleMultiplier,
            intersection.Height * scaleMultiplier);

        spriteBatch.Draw(
            texture,
            destinationRectangle,
            intersection,
            0.9375f);
    }
}

public sealed class NineSliceTopRightCornerRenderer : NineSliceSubRenderer
{
    private readonly int _sliceRight;
    private readonly int _sliceTop;

    public override bool IsEmpty => _sliceRight == 0 || _sliceTop == 0;

    public NineSliceTopRightCornerRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceRight,
        int sliceTop)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceRight = sliceRight;
        _sliceTop = sliceTop;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        var w = SpriteWidth - _sliceRight;
        var subRectangle = new Rectangle(
            _sliceRight,
            0,
            w,
            _sliceTop);
        var xRectangle = new Rectangle(
            RectangularLevelRegion.W - SpriteWidth,
            0,
            w,
            _sliceTop);

        Rectangle.Intersect(ref sourceRectangle, ref xRectangle, out var intersection);
        if (intersection.IsEmpty)
            return;

        var destinationRectangle = new Rectangle(
            screenX - ((subRectangle.X - intersection.X) * scaleMultiplier),
            screenY - ((subRectangle.Y - intersection.Y) * scaleMultiplier),
            intersection.Width * scaleMultiplier,
            intersection.Height * scaleMultiplier);

        spriteBatch.Draw(
            texture,
            destinationRectangle,
            subRectangle,
            0.9375f);
    }
}

public sealed class NineSliceBottomLeftCornerRenderer : NineSliceSubRenderer
{
    private readonly int _sliceLeft;
    private readonly int _sliceBottom;

    public override bool IsEmpty => _sliceLeft == 0 || _sliceBottom == 0;

    public NineSliceBottomLeftCornerRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceLeft,
        int sliceBottom)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceLeft = sliceLeft;
        _sliceBottom = sliceBottom;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        var spriteClip = RectangularLevelRegion.ToRectangle();
        Rectangle.Intersect(ref sourceRectangle, ref spriteClip, out var intersection);
        if (intersection.IsEmpty)
            return;

        var subRectangle = new Rectangle(
            0,
            RectangularLevelRegion.Y - _sliceBottom,
            _sliceLeft,
            _sliceBottom);

        var destinationRectangle = new Rectangle(
            screenX - (intersection.X * scaleMultiplier),
            screenY - (intersection.Y * scaleMultiplier),
            intersection.Width * scaleMultiplier,
            intersection.Height * scaleMultiplier);

        spriteBatch.Draw(
            texture,
            destinationRectangle,
            intersection,
            0.9375f);
    }
}

public sealed class NineSliceBottomRightCornerRenderer : NineSliceSubRenderer
{
    private readonly int _sliceRight;
    private readonly int _sliceBottom;

    public override bool IsEmpty => _sliceRight == 0 || _sliceBottom == 0;

    public NineSliceBottomRightCornerRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceRight,
        int sliceBottom)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceRight = sliceRight;
        _sliceBottom = sliceBottom;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        var subRectangle = new Rectangle(
            RectangularLevelRegion.X - _sliceRight,
            RectangularLevelRegion.Y - _sliceBottom,
            SpriteWidth - _sliceRight,
            SpriteHeight - _sliceBottom);
        Rectangle.Intersect(ref sourceRectangle, ref subRectangle, out var intersection);
        if (intersection.IsEmpty)
            return;

        var destinationRectangle = new Rectangle(
            screenX - (intersection.X * scaleMultiplier),
            screenY - (intersection.Y * scaleMultiplier),
            intersection.Width * scaleMultiplier,
            intersection.Height * scaleMultiplier);

        spriteBatch.Draw(
            texture,
            destinationRectangle,
            intersection,
            0.9375f);
    }
}