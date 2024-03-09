using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
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

    public void LoadGadgetSprite(
        IGadgetBuilder gadgetBuilder,
        GadgetData gadgetData)
    {
        var key = new TextureLookupKey(gadgetBuilder.GadgetBuilderId, gadgetData.Orientation, gadgetData.FacingDirection);

        ref var texture = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetTextures, key, out var exists);
        if (exists)
            return;

        if (gadgetData.Orientation == DownOrientation.Instance &&
            gadgetData.FacingDirection == FacingDirection.RightInstance)
        {
            texture = gadgetBuilder.Sprite;
        }
        else
        {
       /*     var x = _gadgetSpriteCreator.CreateSpriteType(
                gadgetBuilder.Sprite,
                gadgetData.Orientation,
                gadgetData.FacingDirection,
                );*/
        }
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