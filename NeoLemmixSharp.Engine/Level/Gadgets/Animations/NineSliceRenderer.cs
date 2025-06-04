using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class NineSliceRenderer
{
    private readonly RectangularRegion _sourceRegion;

    public NineSliceRenderer(RectangularRegion sourceRegion)
    {
        _sourceRegion = sourceRegion;
    }

    public void Render(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        int layer,
        int currentFrame,
        Color color)
    {
        throw new NotImplementedException();
    }
}
