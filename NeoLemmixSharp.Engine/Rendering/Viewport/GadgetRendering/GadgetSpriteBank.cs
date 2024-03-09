using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetSpriteBank : IDisposable
{
    private readonly Texture2D[] _textures;

    public GadgetSpriteBank(Texture2D[] textures)
    {
        _textures = textures;
    }

    public void Dispose()
    {
        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<Texture2D>(_textures));
    }
}