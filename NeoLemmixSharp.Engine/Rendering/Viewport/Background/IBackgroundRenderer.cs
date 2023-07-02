using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Background;

public interface IBackgroundRenderer
{
    void RenderBackground(SpriteBatch spriteBatch);
}