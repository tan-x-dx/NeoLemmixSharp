using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Background;

public interface IBackgroundRenderer : IDisposable
{
    void RenderBackground(SpriteBatch spriteBatch);
}