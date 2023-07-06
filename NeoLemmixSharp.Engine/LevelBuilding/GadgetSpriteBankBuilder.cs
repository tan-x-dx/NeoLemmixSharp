using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;
using NeoLemmixSharp.Io.LevelReading.Data;
using NeoLemmixSharp.Io.LevelReading.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class GadgetSpriteBankBuilder : IDisposable
{
    private const string _rootDirectory = "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5";

    private readonly GraphicsDevice _graphicsDevice;

    private readonly Dictionary<string, PixelColourData> _textureBundleCache = new();

    public GadgetSpriteBankBuilder(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public void LoadGadgetSprite(GadgetData gadgetData)
    {
        GetOrLoadPixelColourData(gadgetData);
    }

    private PixelColourData GetOrLoadPixelColourData(GadgetData gadgetData)
    {
        var rootFilePath = Path.Combine(_rootDirectory, "styles", gadgetData.Style, "objects", gadgetData.Piece);

        if (_textureBundleCache.TryGetValue(rootFilePath, out var result))
            return result;

        var png = Path.ChangeExtension(rootFilePath, "png");

        using var mainTexture = Texture2D.FromFile(_graphicsDevice, png);
        result = PixelColourData.GetPixelColourDataFromTexture(mainTexture);
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