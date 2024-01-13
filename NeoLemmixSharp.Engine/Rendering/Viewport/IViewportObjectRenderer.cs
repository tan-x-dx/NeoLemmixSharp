using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public interface IViewportObjectRenderer : IDisposable
{
    Rectangle GetSpriteBounds();

    void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY);

}