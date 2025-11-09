using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;

internal sealed class NeoLemmixGadgetAnimationData
{
    public string? TextureFilePath { get; set; }

    public GadgetAnimationBehaviour GadgetAnimationBehaviour { get; set; } = GadgetAnimationBehaviour.LoopToZero;

    public int FrameCount { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}
