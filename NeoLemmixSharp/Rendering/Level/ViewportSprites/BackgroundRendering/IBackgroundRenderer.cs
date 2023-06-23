using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.Level.ViewportSprites.BackgroundRendering;

public interface IBackgroundRenderer
{
    void RenderBackground(SpriteBatch spriteBatch);
}