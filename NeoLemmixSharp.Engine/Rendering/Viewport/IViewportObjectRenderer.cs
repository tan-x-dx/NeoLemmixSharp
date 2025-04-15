using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public interface IViewportObjectRenderer : IRectangularBounds, IDisposable
{
    int RendererId { get; set; }
    int ItemId { get; }

    Rectangle GetSpriteBounds();

    void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY);
}