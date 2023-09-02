using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;

public sealed class GadgetSpriteBank : IDisposable
{
    private readonly Dictionary<string, Texture2D> _textureLookup;

    public GadgetSpriteBank(Dictionary<string, Texture2D> textureLookup)
    {
        _textureLookup = textureLookup;
    }

    public void Dispose()
    {
    }

    public Texture2D GetTexture(string textureName)
    {
        return _textureLookup[textureName];
    }
}