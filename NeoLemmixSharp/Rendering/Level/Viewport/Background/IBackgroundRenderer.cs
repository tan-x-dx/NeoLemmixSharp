using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Background;

public interface IBackgroundRenderer
{
    void RenderBackground(SpriteBatch spriteBatch);
}