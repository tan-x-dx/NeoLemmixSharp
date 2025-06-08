namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct AnimationLayerParameters(int frameStart, int frameEnd, int frameDelta, int transitionToFrame)
{
    internal readonly int FrameStart = frameStart;
    internal readonly int FrameEnd = frameEnd;
    internal readonly int FrameDelta = frameDelta;
    internal readonly int TransitionToFrame = transitionToFrame;

    public int GetTransitionToFrame()
    {
        return TransitionToFrame < 0
            ? FrameStart
            : TransitionToFrame;
    }

    public int GetNextFame(int currentFrame, out bool isEndOfAnimation)
    {
        if (currentFrame == FrameEnd)
        {
            isEndOfAnimation = false;
            return FrameStart;
        }

        currentFrame += FrameDelta;
        isEndOfAnimation = currentFrame == FrameEnd;
        return currentFrame;
    }
}
