using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering;

public interface IRenderable : IDisposable
{
    void Render(SpriteBatch spriteBatch);
}