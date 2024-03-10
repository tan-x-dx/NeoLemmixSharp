using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;
using System.Runtime.InteropServices;
using GadgetSpriteCreator = NeoLemmixSharp.Engine.Rendering.Viewport.SpriteRotationReflectionProcessor<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class GadgetSpriteBuilder : IDisposable
{
    private readonly Dictionary<TextureLookupKey, Texture2D> _gadgetTextures = new(new TextureLookupKeyEqualityComparer());
    private readonly GadgetSpriteCreator _gadgetSpriteCreator;

    public GadgetSpriteBuilder(GraphicsDevice graphicsDevice)
    {
        _gadgetSpriteCreator = new GadgetSpriteCreator(graphicsDevice);
    }

    public IGadgetRenderer? BuildGadgetRenderer(
        IGadgetBuilder gadgetBuilder,
        GadgetData gadgetData)
    {
        // Need to ensure the base texture is placed into the resulting GadgetSpriteBank.
        // If not, the texture will not be disposed of - potentially leading to memory leaks!
        GetOrAddCachedTexture(
            gadgetBuilder,
            DownOrientation.Instance,
            FacingDirection.RightInstance);

        if (!gadgetData.ShouldRender)
            return null;

        var texture = GetOrAddCachedTexture(
            gadgetBuilder,
            gadgetData.Orientation,
            gadgetData.FacingDirection);

        if (gadgetBuilder is ResizeableGadgetBuilder resizeableGadgetBuilder)
            return BuildNineSliceRenderer(resizeableGadgetBuilder, gadgetData, texture!);

        return BuildGenericGadgetRenderer(gadgetBuilder, gadgetData, texture!);
    }

    private Texture2D GetOrAddCachedTexture(
        IGadgetBuilder gadgetBuilder,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        var key = new TextureLookupKey(
            gadgetBuilder.GadgetBuilderId,
            orientation,
            facingDirection);

        ref var texture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);

        if (exists)
            return texture!;

        texture = orientation == DownOrientation.Instance &&
                  facingDirection == FacingDirection.RightInstance // Down orientation + Right facing is the default. No extra work is required, so just use the texture
            ? gadgetBuilder.SpriteData.Texture
            : GetOrientedTexture(gadgetBuilder, orientation, facingDirection);

        return texture;
    }

    private Texture2D GetOrientedTexture(
        IGadgetBuilder gadgetBuilder,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        var spriteData = gadgetBuilder.SpriteData;

        return _gadgetSpriteCreator.CreateSpriteType(
             spriteData.Texture,
             orientation,
             facingDirection,
             spriteData.SpriteWidth,
             spriteData.SpriteHeight,
             spriteData.NumberOfFrames,
             spriteData.NumberOfLayers,
             new LevelPosition(0, 0),
             ItemCreator);
    }

    private NineSliceRenderer BuildNineSliceRenderer(
        ResizeableGadgetBuilder resizeableGadgetBuilder,
        GadgetData gadgetData,
        Texture2D texture)
    {
        throw new NotImplementedException();
    }

    private static GadgetRenderer BuildGenericGadgetRenderer(
        IGadgetBuilder gadgetBuilder,
        GadgetData gadgetData,
        Texture2D texture2D)
    {


        return null;
    }

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
        _gadgetTextures.Values.CopyTo(textureArray, 0);

        return new GadgetSpriteBank(textureArray);
    }

    private readonly struct TextureLookupKey
    {
        public readonly int GadgetBuilderId;
        public readonly Orientation Orientation;
        public readonly FacingDirection FacingDirection;

        public TextureLookupKey(
            int gadgetBuilderId,
            Orientation orientation,
            FacingDirection facingDirection)
        {
            GadgetBuilderId = gadgetBuilderId;
            Orientation = orientation;
            FacingDirection = facingDirection;
        }
    }

    private sealed class TextureLookupKeyEqualityComparer : IEqualityComparer<TextureLookupKey>
    {
        public bool Equals(TextureLookupKey x, TextureLookupKey y)
        {
            return x.GadgetBuilderId == y.GadgetBuilderId &&
                   x.Orientation == y.Orientation &&
                   x.FacingDirection == y.FacingDirection;
        }

        public int GetHashCode(TextureLookupKey key)
        {
            return HashCode.Combine(
                key.GadgetBuilderId,
                key.Orientation,
                key.FacingDirection);
        }
    }

    public void Dispose()
    {
        _gadgetTextures.Clear();
    }
}