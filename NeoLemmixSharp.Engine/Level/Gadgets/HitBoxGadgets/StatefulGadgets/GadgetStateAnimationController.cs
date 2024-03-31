namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetStateAnimationController
{
    public const int NoGadgetStateTransition = -1;

    private readonly GadgetStateAnimationBehaviour _primaryAnimation;
    private readonly GadgetStateAnimationBehaviour? _secondaryAnimation;

    private readonly int _primaryAnimationStateTransitionIndex;
    private readonly GadgetSecondaryAnimationAction _secondaryAnimationAction;

    public GadgetStateAnimationController(
        GadgetStateAnimationBehaviour primaryAnimation,
        int primaryAnimationStateTransitionIndex,
        GadgetStateAnimationBehaviour? secondaryAnimation,
        GadgetSecondaryAnimationAction secondaryAnimationAction)
    {
        _primaryAnimation = primaryAnimation;
        _primaryAnimationStateTransitionIndex = primaryAnimationStateTransitionIndex;
        _secondaryAnimation = secondaryAnimation;
        _secondaryAnimationAction = secondaryAnimationAction;
    }

    public int Tick()
    {
        var result = TickPrimaryAnimation();
        TickSecondaryAnimation();
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

    private void TickSecondaryAnimation()
    {
        if (_secondaryAnimation is null)
            return;

        var newFrame = _secondaryAnimation.CurrentFrame + 1;
        switch (_secondaryAnimationAction)
        {
            case GadgetSecondaryAnimationAction.Play:
                if (newFrame >= _secondaryAnimation.MaxFrame)
                {
                    _secondaryAnimation.CurrentFrame = _secondaryAnimation.MinFrame;
                    return;
                }
                _secondaryAnimation.CurrentFrame = newFrame;
                return;

            case GadgetSecondaryAnimationAction.Pause:
                return;

            case GadgetSecondaryAnimationAction.Stop:
                _secondaryAnimation.CurrentFrame = 0;
                return;

            case GadgetSecondaryAnimationAction.LoopToZero:
                if (_secondaryAnimation.CurrentFrame == _secondaryAnimation.MinFrame)
                    return;

                goto case GadgetSecondaryAnimationAction.Play;

            case GadgetSecondaryAnimationAction.MatchPrimaryAnimationPhysics:
                _secondaryAnimation.CurrentFrame = _primaryAnimation.CurrentFrame;
                return;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnTransitionTo()
    {
        _primaryAnimation.CurrentFrame = _primaryAnimation.MinFrame;

        if (_secondaryAnimation is not null &&
            _secondaryAnimationAction == GadgetSecondaryAnimationAction.MatchPrimaryAnimationPhysics)
        {
            _secondaryAnimation.CurrentFrame = _primaryAnimation.CurrentFrame;
        }
    }
}