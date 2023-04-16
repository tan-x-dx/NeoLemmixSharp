using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering;

public interface IRenderable : IDisposable
{
    Texture2D RenderTexture { get; }
    Rectangle GetLocationRectangle();
    Rectangle GetTextureSourceRectangle();
}