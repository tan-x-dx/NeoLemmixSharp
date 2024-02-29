using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class LayerRenderer<T>
    where T : class
{
    private static readonly GetColor GetWhite = _ => Color.White;

    public delegate Color GetColor(T item);

    private readonly Texture2D _texture;

    private readonly int _layerOffsetX;
    private readonly GetColor _getColor;

    public LayerRenderer(
        Texture2D texture,
        int layerOffsetX)
    {
        _texture = texture;
        _layerOffsetX = layerOffsetX;
        _getColor = GetWhite;
    }

    public LayerRenderer(
        Texture2D texture,
        int layerOffsetX,
        GetColor getColor)
    {
        _texture = texture;
        _layerOffsetX = layerOffsetX;
        _getColor = getColor;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        T item,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        var color = _getColor(item);
        sourceRectangle.X = _layerOffsetX;

        spriteBatch.Draw(
            _texture,
            destinationRectangle,
            sourceRectangle,
            color,
            1.0f);
    }
}