using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;

internal sealed class NeoLemmixGadgetAnimationData
{
    public string? TextureFilePath { get; set; }

    public NeoLemmixStateType GadgetAnimationBehaviour { get; set; } = NeoLemmixStateType.Play;

    public bool IsPrimaryAnimationData => TextureFilePath == string.Empty;

    public int FrameCount { get; set; }
    public int InitialFrame { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }

    public List<AnimationTriggerData> AnimationTriggers { get; } = [];
}
