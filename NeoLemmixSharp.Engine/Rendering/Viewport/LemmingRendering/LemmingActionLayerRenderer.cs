using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionLayerRenderer
{
    private readonly Texture2D _texture;

    private readonly int _layerOffsetX;
    private readonly TeamColorChooser _colorChooser;

    public LemmingActionLayerRenderer(Texture2D texture)
        : this(texture, 0, TeamColorChooser.JustWhite)
    {
    }

    public LemmingActionLayerRenderer(
        Texture2D texture,
        int layerOffsetX,
        TeamColorChooser colorChooser)
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
        var color = _colorChooser.ChooseColor(lemming);
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