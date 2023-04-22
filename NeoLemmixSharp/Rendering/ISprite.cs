using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering;

public interface ISprite : IDisposable
{
    Rectangle GetLocationRectangle();

    void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y);
}