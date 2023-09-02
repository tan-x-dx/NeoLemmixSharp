using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;
using NeoLemmixSharp.Io.LevelReading.Data;
using NeoLemmixSharp.Io.LevelReading.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class GadgetSpriteBankBuilder : IDisposable
{
    private readonly RootDirectoryManager _rootDirectoryManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Dictionary<string, PixelColorData> _textureBundleCache = new();

    public GadgetSpriteBankBuilder(GraphicsDevice graphicsDevice, RootDirectoryManager rootDirectoryManager)
    {
        _graphicsDevice = graphicsDevice;
        _rootDirectoryManager = rootDirectoryManager;
    }

    public void LoadGadgetSprite(GadgetData gadgetData)
    {
        GetOrLoadPixelColorData(gadgetData);
    }

    private PixelColorData GetOrLoadPixelColorData(GadgetData gadgetData)
    {
        var rootFilePath = Path.Combine(_rootDirectoryManager.RootDirectory, "styles", gadgetData.Style, "objects", gadgetData.Piece);

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
        throw new NotImplementedException();
    }

    public void Dispose()
    {

    }
}