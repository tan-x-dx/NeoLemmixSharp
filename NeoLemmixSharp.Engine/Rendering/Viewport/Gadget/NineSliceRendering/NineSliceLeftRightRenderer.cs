using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget.NineSliceRendering;

public sealed class NineSliceLeftRenderer : NineSliceSubRenderer
{
    private readonly int _sliceTop;
    private readonly int _sliceBottom;
    private readonly int _sliceLeft;

    public override bool IsEmpty => _sliceLeft == 0;

    public NineSliceLeftRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceTop,
        int sliceBottom,
        int sliceLeft)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceTop = sliceTop;
        _sliceBottom = sliceBottom;
        _sliceLeft = sliceLeft;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        // throw new NotImplementedException();
    }
}

public sealed class NineSliceRightRenderer : NineSliceSubRenderer
{
    private readonly int _sliceTop;
    private readonly int _sliceBottom;
    private readonly int _sliceRight;

    public override bool IsEmpty => _sliceRight == 0;

    public NineSliceRightRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceTop,
        int sliceBottom,
        int sliceRight)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceTop = sliceTop;
        _sliceBottom = sliceBottom;
        _sliceRight = sliceRight;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        // throw new NotImplementedException();
    }
}