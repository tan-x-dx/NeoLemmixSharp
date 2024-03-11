using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionLayerRenderer
{
    private static readonly GetLemmingColor GetWhite = _ => Color.White;

    public delegate Color GetLemmingColor(Lemming item);

    private readonly Texture2D _texture;

    private readonly int _layerOffsetX;
    private readonly GetLemmingColor _getLemmingColor;

    public LemmingActionLayerRenderer(
        Texture2D texture)
        : this(texture, 0, GetWhite)
    {
    }

    public LemmingActionLayerRenderer(
        Texture2D texture,
        int layerOffsetX,
        GetLemmingColor getLemmingColor)
    {
        _texture = texture;
        _layerOffsetX = layerOffsetX;
        _getLemmingColor = getLemmingColor;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        Lemming item,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        var color = _getLemmingColor(item);
        sourceRectangle.X = _layerOffsetX;

        spriteBatch.Draw(
            _texture,
            destinationRectangle,
            sourceRectangle,
            color,
            1.0f);
    }
}