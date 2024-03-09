using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.InteropServices;
using GadgetSpriteCreator = NeoLemmixSharp.Engine.Rendering.Viewport.SpriteRotationReflectionProcessor<NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.GadgetRenderer>;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class GadgetSpriteBankBuilder
{
    private readonly Dictionary<TextureLookupKey, Texture2D> _gadgetTextures = new(new TextureLookupKeyEqualityComparer());
    private readonly GadgetSpriteCreator _gadgetSpriteCreator;

    public GadgetSpriteBankBuilder(GraphicsDevice graphicsDevice)
    {
        _gadgetSpriteCreator = new GadgetSpriteCreator(graphicsDevice);
    }

    public IViewportObjectRenderer? BuildGadgetRenderer(
        IGadgetBuilder gadgetBuilder,
        GadgetData gadgetData)
    {
        if (!gadgetData.ShouldRender)
            return null;

        var key = new TextureLookupKey(gadgetBuilder.GadgetBuilderId, gadgetData.Orientation, gadgetData.FacingDirection);

        ref var texture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);
        if (exists)
            return null;

        var spriteData = gadgetBuilder.SpriteData;

        if (gadgetData.Orientation == DownOrientation.Instance &&
            gadgetData.FacingDirection == FacingDirection.RightInstance)
        {
            texture = spriteData.Texture;
        }
        else
        {
            var x = _gadgetSpriteCreator.CreateSpriteType(
                spriteData.Texture,
                gadgetData.Orientation,
                gadgetData.FacingDirection,
                spriteData.SpriteWidth,
                spriteData.SpriteHeight,
                spriteData.NumberOfFrames,
                spriteData.NumberOfLayers,
                new LevelPosition(0, 0),
                ItemCreator
            );
        }

        return null;
    }

    private GadgetRenderer ItemCreator(Texture2D texture, int spriteWidth, int spriteHeight, int numberOfFrames, LevelPosition anchorpoint)
    {
        throw new NotImplementedException();
    }

    public GadgetSpriteBank BuildGadgetSpriteBank()
    {
        return new GadgetSpriteBank(_gadgetTextures.Values.ToArray());
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
}