using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering2.Level.ViewportSprites.BackgroundRendering;

public interface IBackgroundRenderer
{
    void RenderBackground(SpriteBatch spriteBatch);
}