using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class GadgetSpriteBuilder : IDisposable
{
    private readonly Dictionary<StylePiecePair, Texture2D> _gadgetTextures;

    public GadgetSpriteBuilder(GraphicsDevice graphicsDevice)
    {
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

        var texture = GetOrAddCachedTexture(
            gadgetData.Orientation,
            gadgetData.FacingDirection);

        return texture;

        Texture2D GetOrAddCachedTexture(
            Orientation orientation,
            FacingDirection facingDirection)
        {
            var key = new StylePiecePair(gadgetArchetypeBuilder.StyleName, gadgetArchetypeBuilder.PieceName);

            ref var cachedTexture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);

            if (exists)
                return cachedTexture!;

            cachedTexture = gadgetArchetypeBuilder.SpriteData.Texture;

            return cachedTexture;
        }
    }

    /*private NineSliceRenderer BuildNineSliceRenderer(
        ResizeableGadgetBuilder resizeableGadgetBuilder,
        GadgetData gadgetData,
        Texture2D texture)
    {


        return new NineSliceRenderer(texture, gadgetData.GadgetRenderMode);
    }*/

    private static Texture2D ItemCreator(
        Texture2D texture,
        Point anchorpoint,
        Size s,
        int numberOfFrames)
    {
        return texture;
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