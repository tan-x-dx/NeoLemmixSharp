using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ControlPanelSpriteBank : IDisposable
{
    private readonly Texture2D[] _textureLookup;

    public ControlPanelSpriteBank(Texture2D[] textureLookup)
    {
        _textureLookup = textureLookup;
    }

    public Texture2D GetTexture(ControlPanelTexture textureName)
    {
        return _textureLookup[(int)textureName];
    }

    public void Dispose()
    {
        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<Texture2D>(_textureLookup));
    }
}