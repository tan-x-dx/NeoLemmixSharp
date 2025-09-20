using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public sealed class GadgetRendererBuilder
{
    private readonly Dictionary<StylePiecePair, Texture2D> _gadgetTextures = new(IoConstants.AssumedNumberOfGadgetArchetypeDataInLevel);

    public GadgetRenderer? BuildStatefulGadgetRenderer(GadgetInstanceData gadgetInstanceData)
    {
        var texture = GetTextureForGadget(gadgetInstanceData);
        return texture is null
            ? null
            : new GadgetRenderer(null, texture, gadgetInstanceData.GadgetRenderMode);
    }

    private Texture2D? GetTextureForGadget(GadgetInstanceData gadgetInstanceData)
    {
        if (gadgetInstanceData.GadgetRenderMode == GadgetRenderMode.NoRender)
            return null;

        var texture = GetOrAddCachedTexture();

        return texture;

        Texture2D GetOrAddCachedTexture()
        {
            var key = gadgetInstanceData.GetStylePiecePair();

            ref var cachedTexture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);

            if (exists)
                return cachedTexture!;

            cachedTexture = LoadSprite(key);

            //AssertSpriteDimensionsMakeSense(gadgetArchetypeBuilder, new Size(cachedTexture.Width, cachedTexture.Height));

            return cachedTexture;
        }
    }

    private static Texture2D LoadSprite(StylePiecePair stylePiecePair)
    {
        var rootFilePath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            stylePiecePair.StyleIdentifier.ToString(),
            DefaultFileExtensions.GadgetFolderName,
            stylePiecePair.PieceIdentifier.ToString());

        var pngPath = Path.ChangeExtension(rootFilePath, "png");

        return TextureCache.GetOrLoadTexture(
            pngPath,
            stylePiecePair.StyleIdentifier,
            stylePiecePair.PieceIdentifier,
            TextureType.GadgetSprite);
    }

    /*private static void AssertSpriteDimensionsMakeSense(StylePiecePair gadgetArchetypeBuilder, Size textureDimensions)
    {
        var spriteData = gadgetArchetypeBuilder.SpriteData;

        var expectedTextureDimensions = new Size(
            spriteData.BaseSpriteSize.W * spriteData.NumberOfLayers,
            spriteData.BaseSpriteSize.H * spriteData.MaxNumberOfFrames);

        if (textureDimensions != expectedTextureDimensions)
            throw new InvalidOperationException("Sprite dimensions are invalid!");
    }*/
}
