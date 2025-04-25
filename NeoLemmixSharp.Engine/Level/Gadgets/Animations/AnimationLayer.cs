using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class AnimationLayer
{
    private readonly AnimationLayerParameters _animationLayerParameters;
    private readonly NineSliceDataThing[] _nineSliceData;

    private readonly int _nextGadgetState;
    private int _currentFrame;
    private bool _isEndOfAnimation;

    public int NextGadgetState => _nextGadgetState;
    public bool IsEndOfAnimation => _isEndOfAnimation;

    public AnimationLayer(
        AnimationLayerParameters animationLayerParameters,
        NineSliceDataThing[] nineSliceData,
        int initialFrame,
        int nextGadgetState)
    {
        _animationLayerParameters = animationLayerParameters;
        _nineSliceData = nineSliceData;
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
        for (var i = 0; i < _nineSliceData.Length; i++)
        {
            _nineSliceData[i].Render(_currentFrame, spriteBatch, texture, sourceRectangle, destinationRectangle);
        }
    }
}
