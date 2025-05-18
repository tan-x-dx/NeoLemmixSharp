using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionLayerRenderer
{
    private readonly Texture2D _texture;

    private readonly int _layerOffsetX;
    private readonly LemmingActionSpriteLayerColorType _colorChooser;

    public LemmingActionLayerRenderer(Texture2D texture)
        : this(texture, 0, LemmingActionSpriteLayerColorType.TrueColor)
    {
    }

    public LemmingActionLayerRenderer(
        Texture2D texture,
        int layerOffsetX,
        LemmingActionSpriteLayerColorType colorChooser)
    {
        _texture = texture;
        _layerOffsetX = layerOffsetX;
        _colorChooser = colorChooser;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        LemmingState lemmingState,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        float rotationAngle,
        SpriteEffects spriteEffects)
    {
        var color = GetColorForLayer(lemmingState);
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

    private Color GetColorForLayer(LemmingState lemmingState) => _colorChooser switch
    {
        LemmingActionSpriteLayerColorType.TrueColor => Color.White,
        LemmingActionSpriteLayerColorType.LemmingHairColor => lemmingState.HairColor,
        LemmingActionSpriteLayerColorType.LemmingSkinColor => lemmingState.SkinColor,
        LemmingActionSpriteLayerColorType.LemmingBodyColor => lemmingState.BodyColor,
        LemmingActionSpriteLayerColorType.LemmingFootColor => lemmingState.FootColor,
        LemmingActionSpriteLayerColorType.TribePaintColor => lemmingState.PaintColor,

        _ => Helpers.ThrowUnknownEnumValueException<LemmingActionSpriteLayerColorType, Color>(_colorChooser)
    };
}