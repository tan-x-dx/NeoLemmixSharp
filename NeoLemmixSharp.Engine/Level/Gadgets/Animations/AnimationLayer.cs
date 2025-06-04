using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class AnimationLayer
{
    private readonly AnimationLayerParameters _animationLayerParameters;
    private readonly NineSliceRenderer[] _nineSliceData;
    private readonly int _layer;
    private readonly Color _color;

    private readonly int _nextGadgetState;
    private int _currentFrame;
    private bool _isEndOfAnimation;

    public int NextGadgetState => _nextGadgetState;
    public bool IsEndOfAnimation => _isEndOfAnimation;

    public AnimationLayer(
        AnimationLayerParameters animationLayerParameters,
        NineSliceRenderer[] nineSliceData,
        int layer,
        Color color,
        int initialFrame,
        int nextGadgetState)
    {
        _animationLayerParameters = animationLayerParameters;
        _nineSliceData = nineSliceData;
        _layer = layer;
        _color = color;
        _currentFrame = initialFrame;
        _nextGadgetState = nextGadgetState;
    }

    public void OnTransitionTo()
    {
        _currentFrame = _animationLayerParameters.GetTransitionToFrame();
    }

    public void Tick()
    {
        _currentFrame = _animationLayerParameters.GetNextFame(_currentFrame, out _isEndOfAnimation);
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        foreach (var renderer in _nineSliceData)
        {
            renderer.Render(
                spriteBatch,
                texture,
                sourceRectangle,
                destinationRectangle,
                _layer,
                _currentFrame,
                _color);
        }
    }
}
