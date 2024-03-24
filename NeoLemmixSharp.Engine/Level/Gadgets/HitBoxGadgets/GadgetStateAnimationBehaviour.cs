namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetStateAnimationBehaviour
{
    public const int NoGadgetStateTransition = -1;

    private readonly int _sourceDx;
    private readonly int _spriteHeight;
    private readonly int _minFrame;
    private readonly int _maxFrame;
    private readonly int _frameDelta;
    private readonly int _gadgetStateTransitionIndex;

    private int _currentFrame;

    public GadgetStateAnimationBehaviour(
         int spriteWidth,
         int spriteHeight,
         int layer,
         int initialFrame,
         int minFrame,
         int maxFrame,
         int frameDelta,
         int gadgetStateTransitionIndex)
    {
        _sourceDx = spriteWidth * layer;
        _spriteHeight = spriteHeight;
        _minFrame = minFrame;
        _maxFrame = maxFrame;
        _frameDelta = frameDelta;
        _gadgetStateTransitionIndex = gadgetStateTransitionIndex;

        _currentFrame = initialFrame;
    }

    public FrameAndLayerData GetFrameData()
    {
        return new FrameAndLayerData(
            _sourceDx,
            _currentFrame * _spriteHeight);
    }

    public int Tick()
    {
        var newFrame = _currentFrame + _frameDelta;
        if (newFrame < _minFrame)
        {
            _currentFrame = _maxFrame - 1;
            return _gadgetStateTransitionIndex;
        }
        if (newFrame >= _maxFrame)
        {
            _currentFrame = _minFrame;
            return _gadgetStateTransitionIndex;
        }
        _currentFrame = newFrame;

        return NoGadgetStateTransition;
    }
}

public readonly ref struct FrameAndLayerData
{
    public readonly int SourceDx;
    public readonly int SourceDy;

    public FrameAndLayerData(int sourceDx, int sourceDy)
    {
        SourceDx = sourceDx;
        SourceDy = sourceDy;
    }
}