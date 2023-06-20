using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering2.Text;

public interface INeoLemmixFont : IDisposable
{
    void RenderText(
        SpriteBatch spriteBatch,
        IEnumerable<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier = 1);
}