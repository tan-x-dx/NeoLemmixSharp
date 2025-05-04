using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public sealed class GadgetRendererBuilder : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Dictionary<StylePiecePair, Texture2D> _gadgetTextures;

    public GadgetRendererBuilder(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _gadgetTextures = new Dictionary<StylePiecePair, Texture2D>(new StylePiecePairEqualityComparer());
    }

    public GadgetRenderer? BuildStatefulGadgetRenderer(
        IGadgetArchetypeBuilder gadgetArchetypeBuilder,
        GadgetData gadgetData)
    {
        var texture = GetTextureForGadget(gadgetArchetypeBuilder, gadgetData);
        return texture is null
            ? null
            : new GadgetRenderer(null, texture, gadgetData.GadgetRenderMode);
    }

    private Texture2D? GetTextureForGadget(
        IGadgetArchetypeBuilder gadgetArchetypeBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.GadgetRenderMode == GadgetRenderMode.NoRender)
            return null;

        var texture = GetOrAddCachedTexture();

        return texture;

        Texture2D GetOrAddCachedTexture()
        {
            var key = new StylePiecePair(gadgetArchetypeBuilder.StyleName, gadgetArchetypeBuilder.PieceName);

            ref var cachedTexture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);

            if (exists)
                return cachedTexture!;

            cachedTexture = LoadSprite(gadgetArchetypeBuilder);

            AssertSpriteDimensionsMakeSense(gadgetArchetypeBuilder, new Size(cachedTexture.Width, cachedTexture.Height));

            return cachedTexture;
        }
    }

    private Texture2D LoadSprite(IGadgetArchetypeBuilder gadgetArchetypeBuilder)
    {
        var rootFilePath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            gadgetArchetypeBuilder.StyleName,
            DefaultFileExtensions.GadgetFolderName,
            gadgetArchetypeBuilder.PieceName);

        var pngPath = Path.ChangeExtension(rootFilePath, "png");

        return Texture2D.FromFile(_graphicsDevice, pngPath);
    }

    private static void AssertSpriteDimensionsMakeSense(IGadgetArchetypeBuilder gadgetArchetypeBuilder, Size textureDimensions)
    {
        var spriteData = gadgetArchetypeBuilder.SpriteData;

        var expectedTextureDimensions = new Size(
            spriteData.BaseSpriteSize.W * spriteData.NumberOfLayers,
            spriteData.BaseSpriteSize.H * spriteData.MaxNumberOfFrames);

        if (textureDimensions != expectedTextureDimensions)
            throw new InvalidOperationException("Sprite dimensions are invalid!");
    }

    public GadgetSpriteBank BuildGadgetSpriteBank()
    {
        var textureArray = new Texture2D[_gadgetTextures.Count];
        var i = 0;
        foreach (var (_, gadgetTexture) in _gadgetTextures)
        {
            textureArray[i++] = gadgetTexture;
        }

        return new GadgetSpriteBank(textureArray);
    }

    public void Dispose()
    {
        _gadgetTextures.Clear();
    }
}