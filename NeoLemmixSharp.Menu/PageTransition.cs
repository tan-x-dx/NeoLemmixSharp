using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Menu;

public sealed class PageTransition
{
    private const double TransitionAlphaOffset = 1d / 256d;
    private const double TransitionDelta = 1d / EngineConstants.PageTransitionDurationInFrames;

    private double _transitionAlpha;
    private int _transitionCount = -1;

    public int TransitionAlpha => Math.Clamp((int)(_transitionAlpha * 255d), 0x00, 0xff);

    public bool IsTransitioning => _transitionCount >= 0;
    public bool IsHalfWayDone => _transitionCount == EngineConstants.PageTransitionDurationInFrames + 1;
    public bool IsDone => _transitionCount == 1 + (EngineConstants.PageTransitionDurationInFrames * 2);

    public void BeginTransition()
    {
        _transitionCount = 0;
        _transitionAlpha = TransitionAlphaOffset;
    }

    public void Tick()
    {
        if (!IsTransitioning)
            return;

        _transitionCount++;
        if (_transitionCount <= EngineConstants.PageTransitionDurationInFrames)
        {
            _transitionAlpha = Math.Min(_transitionAlpha + TransitionDelta, 1d);

            return;
        }

        if (_transitionCount == EngineConstants.PageTransitionDurationInFrames + 1)
        {
            _transitionAlpha = 1d - TransitionAlphaOffset;
        }

        if (_transitionCount > EngineConstants.PageTransitionDurationInFrames + 1)
        {
            if (IsDone)
            {
                _transitionCount = -1;
                _transitionAlpha = 0d;
                return;
            }

            _transitionAlpha = Math.Max(_transitionAlpha - TransitionDelta, 0d);
        }
    }
}
