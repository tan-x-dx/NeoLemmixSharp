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
        TribeSpriteLayerColorType.NoRender => Color.Transparent,
        TribeSpriteLayerColorType.TrueColor => Color.White,
        TribeSpriteLayerColorType.LemmingHairColor => lemmingState.HairColor,
        TribeSpriteLayerColorType.LemmingSkinColor => lemmingState.SkinColor,
        TribeSpriteLayerColorType.LemmingBodyColor => lemmingState.BodyColor,
        TribeSpriteLayerColorType.LemmingFootColor => lemmingState.FootColor,
        TribeSpriteLayerColorType.TribePaintColor => lemmingState.PaintColor,

        _ => Helpers.ThrowUnknownEnumValueException<TribeSpriteLayerColorType, Color>(_colorChooser)
    };
}