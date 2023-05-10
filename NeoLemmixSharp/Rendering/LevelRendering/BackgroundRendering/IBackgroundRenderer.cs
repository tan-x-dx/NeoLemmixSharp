using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.LevelRendering.BackgroundRendering;

public interface IBackgroundRenderer
{
    void RenderBackground(SpriteBatch spriteBatch);
}