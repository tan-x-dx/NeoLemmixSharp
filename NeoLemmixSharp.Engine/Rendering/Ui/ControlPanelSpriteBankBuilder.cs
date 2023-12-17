using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ControlPanelSpriteBankBuilder
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Texture2D[] _textureLookup;

    public ControlPanelSpriteBankBuilder(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _contentManager = contentManager;

        var numberOfResources = Enum.GetValuesAsUnderlyingType<ControlPanelTexture>().Length;
        _textureLookup = new Texture2D[numberOfResources];
    }

    public ControlPanelSpriteBank BuildControlPanelSpriteBank()
    {
        CreateAnchorTexture();
        CreateWhitePixelTexture();
        LoadPanelTextures();
        LoadCursorSprites();

        return new ControlPanelSpriteBank(_textureLookup);
    }

    private void CreateAnchorTexture()
    {
        var anchorTexture = new Texture2D(_graphicsDevice, 3, 3)
        {
            Name = ControlPanelTexture.LemmingAnchorTexture.GetTexturePath()
        };

        var red = new Color(200, 0, 0, 255).PackedValue;
        var yellow = new Color(200, 200, 0, 255).PackedValue;

        var x = new uint[9];
        x[1] = red;
        x[3] = red;
        x[4] = yellow;
        x[5] = red;
        x[7] = red;
        anchorTexture.SetData(x);

        RegisterTexture(ControlPanelTexture.LemmingAnchorTexture, anchorTexture);
    }

    private void CreateWhitePixelTexture()
    {
        var whitePixelTexture = new Texture2D(_graphicsDevice, 1, 256)
        {
            Name = ControlPanelTexture.WhitePixel.GetTexturePath()
        };

        var whiteColors = Enumerable
            .Range(0, 256)
            .Select(alpha => new Color(0xff, 0xff, 0xff, 255 - alpha))
            .ToArray();

        whitePixelTexture.SetData(whiteColors);
        RegisterTexture(ControlPanelTexture.WhitePixel, whitePixelTexture);
    }

    private void LoadPanelTextures()
    {
        RegisterTexture(ControlPanelTexture.Panel);
        RegisterTexture(ControlPanelTexture.PanelMinimapRegion);
        RegisterTexture(ControlPanelTexture.PanelIcons);
        RegisterTexture(ControlPanelTexture.PanelSkillSelected);
        RegisterTexture(ControlPanelTexture.PanelSkills);
    }

    private void LoadCursorSprites()
    {
        RegisterTexture(ControlPanelTexture.CursorStandard);
        RegisterTexture(ControlPanelTexture.CursorFocused);
    }

    private void RegisterTexture(ControlPanelTexture textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName.GetTexturePath());
        RegisterTexture(textureName, texture);
    }

    private void RegisterTexture(ControlPanelTexture textureName, Texture2D texture)
    {
        _textureLookup[(int)textureName] = texture;
    }
}