using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

public interface IBackgroundRenderer : IDisposable
{
    void RenderBackground(SpriteBatch spriteBatch);
}