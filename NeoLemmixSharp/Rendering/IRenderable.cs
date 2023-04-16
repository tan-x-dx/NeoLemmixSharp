using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering;

public interface IRenderable : IDisposable
{
    Rectangle GetLocationRectangle();

    void RenderAtPosition(SpriteBatch spriteBatch, int x, int y);
}