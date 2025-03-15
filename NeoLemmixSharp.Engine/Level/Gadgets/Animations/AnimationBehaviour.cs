using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class AnimationBehaviour
{
    private readonly AnimationParameters _animationParameters;
    private readonly NineSliceDataThing[] _nineSliceData;

    private int _currentFrame;
    private int _nextGadgetState;
    private bool _isEndOfAnimation;

    public int NextGadgetState => _nextGadgetState;
    public bool IsEndOfAnimation => _isEndOfAnimation;

    public AnimationBehaviour(
        AnimationParameters animationParameters,
        NineSliceDataThing[] nineSliceData,
        int initialFrame,
        int nextGadgetState)
    {
        _animationParameters = animationParameters;
        _nineSliceData = nineSliceData;
        _currentFrame = initialFrame;
        _nextGadgetState = nextGadgetState;
    }

    public void OnTransitionTo()
    {
        _currentFrame = _animationParameters.GetTransitionToFrame();
    }

    public void Tick()
    {
        _currentFrame = _animationParameters.GetNextFame(_currentFrame, out _isEndOfAnimation);
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
