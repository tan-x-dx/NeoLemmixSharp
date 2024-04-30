namespace NeoLemmixSharp.Menu;

public sealed class PageTransition
{
    private const double TransitionAlphaOffset = 1d / 256d;

    private readonly int _transitionDurationInFrames;
    private readonly double _transitionDelta;

    private double _transitionAlpha;

    private int _transitionCount = -1;

    public int TransitionAlpha => Math.Clamp((int)(_transitionAlpha * 255d), 0x00, 0xff);

    public bool IsTransitioning => _transitionCount >= 0;
    public bool IsHalfWayDone => _transitionCount == _transitionDurationInFrames + 1;
    public bool IsDone => _transitionCount == 1 + _transitionDurationInFrames * 2;

    public PageTransition(int transitionDurationInFrames)
    {
        _transitionDurationInFrames = transitionDurationInFrames;
        _transitionDelta = 1.0d / transitionDurationInFrames;
    }

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
        if (_transitionCount <= _transitionDurationInFrames)
        {
            _transitionAlpha = Math.Min(_transitionAlpha + _transitionDelta, 1d);

            return;
        }

        if (_transitionCount == _transitionDurationInFrames + 1)
        {
            _transitionAlpha = 1d - TransitionAlphaOffset;
        }

        if (_transitionCount > _transitionDurationInFrames + 1)
        {
            if (IsDone)
            {
                _transitionCount = -1;
                _transitionAlpha = 0d;
                return;
            }

            _transitionAlpha = Math.Max(_transitionAlpha - _transitionDelta, 0d);
        }
    }
}