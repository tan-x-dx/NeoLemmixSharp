using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionLayerRenderer
{
    private readonly Texture2D _texture;

    private readonly int _layerOffsetX;
    private readonly TribeSpriteLayerColorType _colorChooser;

    public LemmingActionLayerRenderer(Texture2D texture)
        : this(texture, 0, TribeSpriteLayerColorType.NoRender)
    {
    }

    public LemmingActionLayerRenderer(
        Texture2D texture,
        int layerOffsetX,
        TribeSpriteLayerColorType colorChooser)
    {
        _texture = texture;
        _layerOffsetX = layerOffsetX;
        _colorChooser = colorChooser;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        float rotationAngle,
        SpriteEffects spriteEffects)
    {
        var color = GetColorForLayer(lemming);
        sourceRectangle.X += _layerOffsetX;

        spriteBatch.Draw(
            _texture,
            destinationRectangle,
            sourceRectangle,
            color,
            rotationAngle,
            Vector2.Zero,
            spriteEffects,
            1.0f);
    }

    private Color GetColorForLayer(Lemming lemming) => _colorChooser switch
    {
        TribeSpriteLayerColorType.NoRender => Color.Transparent,
        TribeSpriteLayerColorType.TrueColor => Color.White,
        TribeSpriteLayerColorType.LemmingHairColor => lemming.LemmingColors.HairColor,
        TribeSpriteLayerColorType.LemmingSkinColor => lemming.LemmingColors.SkinColor,
        TribeSpriteLayerColorType.LemmingBodyColor => lemming.LemmingColors.BodyColor,
        TribeSpriteLayerColorType.LemmingFootColor => lemming.LemmingColors.FootColor,
        TribeSpriteLayerColorType.TribePaintColor =>  lemming.LemmingColors.PaintColor,

        _ => Helpers.ThrowUnknownEnumValueException<TribeSpriteLayerColorType, Color>(_colorChooser)
    };
}
