using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget.NineSliceRendering;

public sealed class NineSliceCentreRenderer : NineSliceSubRenderer
{
    private readonly int _sliceLeft;
    private readonly int _sliceRight;
    private readonly int _sliceTop;
    private readonly int _sliceBottom;

    public override bool IsEmpty => false;

    public NineSliceCentreRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceLeft,
        int sliceRight,
        int sliceTop,
        int sliceBottom)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceLeft = sliceLeft;
        _sliceRight = sliceRight;
        _sliceTop = sliceTop;
        _sliceBottom = sliceBottom;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        //  throw new NotImplementedException();
    }
}