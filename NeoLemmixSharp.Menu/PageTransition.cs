namespace NeoLemmixSharp.Menu;

public sealed class PageTransition
{
    private const float TransitionAlphaOffset = 1f / 256f;

    private readonly int _transitionDurationInFrames;
    private readonly float _transitionDelta;

    private int _transitionCount = -1;

    public float TransitionAlpha { get; private set; }

    public bool IsTransitioning => _transitionCount >= 0;
    public bool IsHalfWayDone => _transitionCount == _transitionDurationInFrames;
    public bool IsDone => _transitionCount == _transitionDurationInFrames * 2;

    public PageTransition(int transitionDurationInFrames)
    {
        _transitionDurationInFrames = transitionDurationInFrames;
        _transitionDelta = 1.0f / transitionDurationInFrames;
    }

    public void BeginTransition()
    {
        _transitionCount = 0;
        TransitionAlpha = TransitionAlphaOffset;
    }

    public void Tick()
    {
        if (!IsTransitioning)
            return;

        _transitionCount++;
        if (_transitionCount < _transitionDurationInFrames)
        {
            TransitionAlpha = Math.Min(TransitionAlpha + _transitionDelta, 1f);
            
            return;
        }

        if (_transitionCount == _transitionDurationInFrames)
        {
            TransitionAlpha = 1f - TransitionAlphaOffset;
        }

        if (_transitionCount > _transitionDurationInFrames)
        {
            if (IsDone)
            {
                _transitionCount = -1;
                TransitionAlpha = 0f;
                return;
            }

            TransitionAlpha = Math.Max(TransitionAlpha - _transitionDelta, 0f);
        }
    }
}