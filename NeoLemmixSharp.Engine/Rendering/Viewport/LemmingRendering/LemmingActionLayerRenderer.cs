using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionLayerRenderer
{
    private static readonly GetLayerColor GetWhite = _ => Color.White;

    public delegate Color GetLayerColor(Lemming lemming);

    private readonly Texture2D _texture;

    private readonly int _layerOffsetX;
    private readonly GetLayerColor _getLayerColor;

    public LemmingActionLayerRenderer(Texture2D texture)
        : this(texture, 0, GetWhite)
    {
    }

    public LemmingActionLayerRenderer(
        Texture2D texture,
        int layerOffsetX,
        GetLayerColor getLemmingColor)
    {
        _texture = texture;
        _layerOffsetX = layerOffsetX;
        _getLayerColor = getLemmingColor;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        float rotationAngle,
        SpriteEffects spriteEffects)
    {
        var color = _getLayerColor(lemming);
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
}