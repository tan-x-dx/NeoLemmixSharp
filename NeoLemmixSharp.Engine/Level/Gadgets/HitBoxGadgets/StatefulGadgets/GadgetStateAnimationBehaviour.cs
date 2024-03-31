namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetStateAnimationBehaviour
{
    private readonly int _sourceDx;
    private readonly int _spriteHeight;
    public int MinFrame { get; }
    public int MaxFrame { get; }

    public int CurrentFrame { get; set; }

    public GadgetStateAnimationBehaviour(
         int spriteWidth,
         int spriteHeight,
         int layer,
         int initialFrame,
         int minFrame,
         int maxFrame)
    {
        _sourceDx = spriteWidth * layer;
        _spriteHeight = spriteHeight;
        MinFrame = minFrame;
        MaxFrame = maxFrame;

        CurrentFrame = initialFrame;
    }

    public FrameAndLayerData GetFrameAndLayerData()
    {
        return new FrameAndLayerData(
            _sourceDx,
            CurrentFrame * _spriteHeight);
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