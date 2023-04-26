using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering.Text;

public interface INeoLemmixFont : IDisposable
{
    void RenderText(
        SpriteBatch spriteBatch,
        IEnumerable<char> charactersToRender,
        int x,
        int y);
}