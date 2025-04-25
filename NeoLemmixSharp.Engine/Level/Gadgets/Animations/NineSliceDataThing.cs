using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Teams;
using static NeoLemmixSharp.Engine.Level.Gadgets.Animations.TeamColors;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class NineSliceDataThing
{
    public NineSliceDataThing()
    {
    }

    public void Render(
        int currentFrame,
        SpriteBatch spriteBatch,
        Texture2D texture,
        TeamColorChooser teamColorChooser,
        Team? team,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        throw new NotImplementedException();
    }
}
