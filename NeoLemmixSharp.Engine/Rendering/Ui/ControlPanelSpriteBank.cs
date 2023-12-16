using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Rendering.Viewport;

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

    public LevelCursorSprite GetLevelCursorSprite(LevelCursor levelCursor)
    {
        return new LevelCursorSprite(
            levelCursor,
            GetTexture(ControlPanelTexture.CursorStandard),
            GetTexture(ControlPanelTexture.CursorFocused));
    }

    public void Dispose()
    {
        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<Texture2D>(_textureLookup));
    }
}