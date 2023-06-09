﻿using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public interface INeoLemmixFont : IDisposable
{
    void RenderText(
        SpriteBatch spriteBatch,
        IEnumerable<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier = 1);
}