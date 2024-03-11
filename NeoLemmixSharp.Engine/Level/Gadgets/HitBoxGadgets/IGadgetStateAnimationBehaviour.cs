namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public interface IGadgetStateAnimationBehaviour
{
    void GetFrameData(out FrameAndLayerData data);

    void Tick();
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

public sealed class CountUpAndLoopAnimationBehaviour : IGadgetStateAnimationBehaviour
{
    private readonly int _sourceDx;
    private readonly int _spriteHeight;
    private readonly int _minFrame;
    private readonly int _maxFrame;

    private int _currentFrame;
    public bool IsActive = true;

    public CountUpAndLoopAnimationBehaviour(
        int spriteWidth,
        int spriteHeight,
        int layer,
        int initialFrame,
        int minFrame,
        int maxFrame)
    {
        _sourceDx = spriteWidth * layer;
        _spriteHeight = spriteHeight;
        _minFrame = minFrame;
        _maxFrame = maxFrame;
        _currentFrame = initialFrame;
    }

    public void GetFrameData(out FrameAndLayerData data)
    {
        data = new FrameAndLayerData(
            _sourceDx,
            _currentFrame * _spriteHeight);
    }

    public void Tick()
    {
        if (!IsActive)
            return;

        var c = _currentFrame + 1;
        if (c >= _maxFrame)
        {
            _currentFrame = _minFrame;
            return;
        }
        _currentFrame = c;
    }
}