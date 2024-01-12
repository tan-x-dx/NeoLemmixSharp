using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetSpriteBank : IDisposable
{
    private readonly Dictionary<string, Texture2D> _textureLookup;

    public GadgetSpriteBank(Dictionary<string, Texture2D> textureLookup)
    {
        _textureLookup = textureLookup;
    }

    public void Dispose()
    {
        DisposableHelperMethods.DisposeOfAll(_textureLookup);
    }

    public Texture2D GetTexture(string textureName)
    {
        return _textureLookup[textureName];
    }
}