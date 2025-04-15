using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class AnimationController
{
    private readonly AnimationBehaviour[] _animations;

    private readonly GadgetBounds _currentGadgetBounds;

    public AnimationController(
        AnimationBehaviour[] animations,
        GadgetBounds currentGadgetBounds)
    {
        if (animations.Length < 1)
            throw new ArgumentException("Requires at least ONE animation behaviour");

        _animations = animations;
        _currentGadgetBounds = currentGadgetBounds;
    }

    public RectangularRegion CurrentBounds => _currentGadgetBounds.CurrentBounds;

    public int GetNextStateIndex()
    {
        var primaryAnimation = _animations[0];
        return primaryAnimation.IsEndOfAnimation
            ? primaryAnimation.NextGadgetState
            : -1;
    }

    public void OnTransitionTo()
    {
        for (var i = 0; i < _animations.Length; i++)
        {
            _animations[i].OnTransitionTo();
        }
    }

    public void Tick()
    {
        for (var i = 0; i < _animations.Length; i++)
        {
            _animations[i].Tick();
        }
    }

    public void RenderLayers(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle)
    {
        for (var i = 0; i < _animations.Length; i++)
        {
            _animations[i].RenderLayer(spriteBatch, texture, sourceRectangle, destinationRectangle);
        }
    }
}
