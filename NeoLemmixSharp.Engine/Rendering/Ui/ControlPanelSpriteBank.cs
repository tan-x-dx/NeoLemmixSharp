using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ControlPanelSpriteBank : IDisposable
{
    private readonly Dictionary<ControlPanelTexture, Texture2D> _textureLookup;

    public ControlPanelSpriteBank(Dictionary<ControlPanelTexture, Texture2D> textureLookup)
    {
        _textureLookup = textureLookup;
    }

    public Texture2D GetTexture(ControlPanelTexture textureName)
    {
        return _textureLookup[textureName];
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
        HelperMethods.DisposeOf(_textureLookup);
    }
}