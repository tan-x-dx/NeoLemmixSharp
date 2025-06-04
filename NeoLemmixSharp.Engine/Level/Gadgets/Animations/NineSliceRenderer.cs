using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class NineSliceRenderer
{
    private readonly RectangularRegion _sourceRegion;

    public NineSliceRenderer(RectangularRegion sourceRegion)
    {
        _sourceRegion = sourceRegion;
    }

    public void Render(
        int currentFrame,
        SpriteBatch spriteBatch,
        Texture2D texture,
        Tribe? tribe,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        throw new NotImplementedException();
    }
}
