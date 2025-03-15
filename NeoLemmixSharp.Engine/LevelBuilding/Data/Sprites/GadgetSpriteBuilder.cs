using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.InteropServices;
using GadgetSpriteCreator = NeoLemmixSharp.Engine.Rendering.Viewport.SpriteRotationReflectionProcessor<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class GadgetSpriteBuilder : IDisposable, IEqualityComparer<GadgetSpriteBuilder.TextureLookupKey>
{
    private readonly GadgetSpriteCreator _gadgetSpriteCreator;
    private readonly Dictionary<TextureLookupKey, Texture2D> _gadgetTextures;

    public GadgetSpriteBuilder(GraphicsDevice graphicsDevice)
    {
        _gadgetSpriteCreator = new GadgetSpriteCreator(graphicsDevice);
        _gadgetTextures = new Dictionary<TextureLookupKey, Texture2D>(this);
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
        // Need to ensure the base texture is placed into the resulting GadgetSpriteBank.
        // If not, the texture will not be disposed of - potentially leading to memory leaks!
        GetOrAddCachedTexture(
            Orientation.Down,
            FacingDirection.Right);

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
            var key = new TextureLookupKey(
                gadgetArchetypeBuilder.StyleName,
                gadgetArchetypeBuilder.PieceName,
                orientation,
                facingDirection);

            ref var cachedTexture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);

            if (exists)
                return cachedTexture!;

            cachedTexture = orientation == Orientation.Down &&
                            facingDirection == FacingDirection.Right // Down orientation + Right facing is the default. No extra work is required, so just use the texture
                ? gadgetArchetypeBuilder.SpriteData.Texture
                : GetOrientedTexture(orientation, facingDirection);

            return cachedTexture;
        }

        Texture2D GetOrientedTexture(
            Orientation orientation,
            FacingDirection facingDirection)
        {
            var spriteData = gadgetArchetypeBuilder.SpriteData;

            var numberOfFrames = spriteData.FrameCountsPerLayer.Max();

            return _gadgetSpriteCreator.CreateSpriteType(
                spriteData.Texture,
                orientation,
                facingDirection,
                spriteData.SpriteWidth,
                spriteData.SpriteHeight,
                numberOfFrames,
                spriteData.NumberOfLayers,
                new LevelPosition(0, 0),
                ItemCreator);
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
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        LevelPosition anchorpoint)
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

    public readonly struct TextureLookupKey(string styleName, string pieceName, Orientation orientation, FacingDirection facingDirection)
    {
        public readonly string StyleName = styleName;
        public readonly string PieceName = pieceName;
        public readonly Orientation Orientation = orientation;
        public readonly FacingDirection FacingDirection = facingDirection;
    }

    public bool Equals(TextureLookupKey x, TextureLookupKey y)
    {
        return x.Orientation == y.Orientation &&
               x.FacingDirection == y.FacingDirection &&
               string.Equals(x.StyleName, y.StyleName) &&
               string.Equals(x.PieceName, y.PieceName);
    }

    public int GetHashCode(TextureLookupKey key)
    {
        return HashCode.Combine(
            key.StyleName,
            key.PieceName,
            key.Orientation,
            key.FacingDirection);
    }

    public void Dispose()
    {
        _gadgetTextures.Clear();
    }
}