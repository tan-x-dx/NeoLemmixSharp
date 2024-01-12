using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.Sprites;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class GadgetSpriteBankBuilder
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Dictionary<string, PixelColorData> _textureBundleCache = new();
    private readonly Dictionary<string, Texture2D> _gadgetSprites = new();

    public GadgetSpriteBankBuilder(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _contentManager = contentManager;
    }

    public void LoadGadgetSprite(NeoLemmixGadgetData gadgetData)
    {
        GetOrLoadPixelColorData(gadgetData);
    }

    private void RegisterTexture(string lookupName, string textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName);
        _gadgetSprites.Add(lookupName, texture);
    }

    private PixelColorData GetOrLoadPixelColorData(NeoLemmixGadgetData gadgetData)
    {
        var rootFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", gadgetData.Style, "objects", gadgetData.Piece);

        if (_textureBundleCache.TryGetValue(rootFilePath, out var result))
            return result;

        var png = Path.ChangeExtension(rootFilePath, "png");

        using var mainTexture = Texture2D.FromFile(_graphicsDevice, png);
        result = PixelColorData.GetPixelColorDataFromTexture(mainTexture);
        _textureBundleCache.Add(rootFilePath, result);

        return result;
    }

    public GadgetSpriteBank BuildGadgetSpriteBank()
    {
        RegisterTexture("switch", "sprites/style/common/switch");
        RegisterTexture("sawblade", "sprites/style/common/spinner");

        return new GadgetSpriteBank(_gadgetSprites);
    }
}