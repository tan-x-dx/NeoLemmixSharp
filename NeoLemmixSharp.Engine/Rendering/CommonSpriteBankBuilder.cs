using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering;

public readonly ref struct CommonSpriteBankBuilder
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Texture2D[] _textureLookup;

    public CommonSpriteBankBuilder(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _contentManager = contentManager;

        var numberOfResources = Enum.GetValuesAsUnderlyingType<CommonTexture>().Length;
        _textureLookup = new Texture2D[numberOfResources];
    }

    public void BuildCommonSpriteBank()
    {
        CreateAnchorTexture();
        CreateWhitePixelTexture();
        LoadCursorSprites();

        CommonSpriteBank.Initialise(_textureLookup);
    }

    private void CreateAnchorTexture()
    {
        var anchorTexture = new Texture2D(_graphicsDevice, 3, 3)
        {
            Name = CommonTexture.LemmingAnchorTexture.GetTexturePath()
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

        RegisterTexture(CommonTexture.LemmingAnchorTexture, anchorTexture);
    }

    private void CreateWhitePixelTexture()
    {
        var whitePixelTexture = new Texture2D(_graphicsDevice, 1, 256)
        {
            Name = CommonTexture.WhitePixel.GetTexturePath()
        };

        var whiteColors = new Color[256];
        for (var i = 0; i < whiteColors.Length; i++)
        {
            whiteColors[i] = new Color(0xff, 0xff, 0xff, i);
        }

        whitePixelTexture.SetData(whiteColors);
        RegisterTexture(CommonTexture.WhitePixel, whitePixelTexture);
    }

    private void LoadCursorSprites()
    {
        RegisterTexture(CommonTexture.LevelCursors);
    }

    private void RegisterTexture(CommonTexture textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName.GetTexturePath());
        RegisterTexture(textureName, texture);
    }

    private void RegisterTexture(CommonTexture textureName, Texture2D texture)
    {
        _textureLookup[(int)textureName] = texture;
    }
}