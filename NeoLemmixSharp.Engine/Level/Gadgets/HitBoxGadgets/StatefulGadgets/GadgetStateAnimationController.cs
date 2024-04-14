using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetStateAnimationController
{
    public const int NoGadgetStateTransition = -1;

    private readonly GadgetStateAnimationBehaviour _primaryAnimation;
    private readonly GadgetStateAnimationBehaviour[] _secondaryAnimations;

    private readonly int _primaryAnimationStateTransitionIndex;

    public GadgetStateAnimationController(
        GadgetStateAnimationBehaviour primaryAnimation,
        int primaryAnimationStateTransitionIndex,
        GadgetStateAnimationBehaviour[] secondaryAnimations)
    {
        _primaryAnimation = primaryAnimation;
        _primaryAnimationStateTransitionIndex = primaryAnimationStateTransitionIndex;
        _secondaryAnimations = secondaryAnimations;
    }

    public void RenderLayers(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        _primaryAnimation.RenderLayer(spriteBatch, texture, sourceRectangle, destinationRectangle);
        foreach (var secondaryAnimation in _secondaryAnimations)
        {
            secondaryAnimation.RenderLayer(spriteBatch, texture, sourceRectangle, destinationRectangle);
        }
    }

    public int Tick()
    {
        var result = TickPrimaryAnimation();
        foreach (var secondaryAnimation in _secondaryAnimations)
        {
            TickSecondaryAnimation(secondaryAnimation);
        }
        return result;
    }

    private int TickPrimaryAnimation()
    {
        var newFrame = _primaryAnimation.CurrentFrame + 1;
        if (newFrame >= _primaryAnimation.MaxFrame)
        {
            _primaryAnimation.CurrentFrame = _primaryAnimation.MinFrame;
            return _primaryAnimationStateTransitionIndex;
        }

        _primaryAnimation.CurrentFrame = newFrame;

        return NoGadgetStateTransition;
    }

    private void TickSecondaryAnimation(GadgetStateAnimationBehaviour secondaryAnimation)
    {
        var newFrame = secondaryAnimation.CurrentFrame + 1;
        switch (secondaryAnimation.SecondaryAnimationAction)
        {
            case GadgetSecondaryAnimationAction.Play:
                if (newFrame >= secondaryAnimation.MaxFrame)
                {
                    secondaryAnimation.CurrentFrame = secondaryAnimation.MinFrame;
                    return;
                }

                secondaryAnimation.CurrentFrame = newFrame;
                return;

            case GadgetSecondaryAnimationAction.Pause:
                return;

            case GadgetSecondaryAnimationAction.Stop:
                secondaryAnimation.CurrentFrame = 0;
                return;

            case GadgetSecondaryAnimationAction.LoopToZero:
                if (secondaryAnimation.CurrentFrame == secondaryAnimation.MinFrame)
                    return;

                goto case GadgetSecondaryAnimationAction.Play;

            case GadgetSecondaryAnimationAction.MatchPrimaryAnimationPhysics:
                secondaryAnimation.CurrentFrame = _primaryAnimation.CurrentFrame;
                return;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnTransitionTo()
    {
        _primaryAnimation.CurrentFrame = _primaryAnimation.MinFrame;

        foreach (var secondaryAnimation in _secondaryAnimations)
        {
            if (secondaryAnimation.SecondaryAnimationAction ==
                GadgetSecondaryAnimationAction.MatchPrimaryAnimationPhysics)
            {
                secondaryAnimation.CurrentFrame = _primaryAnimation.CurrentFrame;
            }
        }
    }
}