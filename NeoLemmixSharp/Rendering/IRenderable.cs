using System;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering;

public interface IRenderable : IDisposable
{
    void Render(SpriteBatch spriteBatch);
}