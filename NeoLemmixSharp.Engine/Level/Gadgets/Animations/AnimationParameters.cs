
namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public readonly struct AnimationParameters(int frameStart, int frameEnd, int frameDelta, int transitionToFrame)
{
    private readonly int _frameStart = frameStart;
    private readonly int _frameEnd = frameEnd;
    private readonly int _frameDelta = frameDelta;
    private readonly int _transitionToFrame = transitionToFrame;

    public int GetTransitionToFrame()
    {
        return _transitionToFrame < 0
            ? _frameStart
            : _transitionToFrame;
    }

    public int GetNextFame(int currentFrame, out bool isEndOfAnimation)
    {
        if (currentFrame == _frameEnd)
        {
            isEndOfAnimation = false;
            return _frameStart;
        }

        currentFrame += _frameDelta;
        isEndOfAnimation = currentFrame == _frameEnd;
        return currentFrame;
    }
}
