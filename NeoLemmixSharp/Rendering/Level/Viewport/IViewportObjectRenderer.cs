using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.Level.Viewport;

public interface IViewportObjectRenderer : IDisposable
{
    Rectangle GetLocationRectangle();

    void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier);
    void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier);

}