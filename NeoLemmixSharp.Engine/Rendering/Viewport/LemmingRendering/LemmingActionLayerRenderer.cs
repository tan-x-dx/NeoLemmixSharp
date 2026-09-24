using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
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
        ref var colorData = ref LevelScreen.LemmingColorManager.GetLemmingColorData(lemming.Id);
        var color = GetColorForLayer(ref colorData);
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

    private Color GetColorForLayer(ref readonly LemmingColorData lemmingColors) => _colorChooser switch
    {
        TribeSpriteLayerColorType.NoRender => Color.Transparent,
        TribeSpriteLayerColorType.TrueColor => Color.White,
        TribeSpriteLayerColorType.LemmingHairColor => lemmingColors.HairColor,
        TribeSpriteLayerColorType.LemmingSkinColor => lemmingColors.SkinColor,
        TribeSpriteLayerColorType.LemmingBodyColor => lemmingColors.BodyColor,
        TribeSpriteLayerColorType.LemmingFootColor => lemmingColors.FootColor,
        TribeSpriteLayerColorType.TribePaintColor => lemmingColors.PaintColor,

        _ => Helpers.ThrowUnknownEnumValueException<TribeSpriteLayerColorType, Color>(_colorChooser)
    };
}
