using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;

internal sealed class NeoLemmixGadgetAnimationData
{
    public string? TextureFilePath { get; set; }

    public NeoLemmixStateType GadgetAnimationBehaviour { get; set; } = NeoLemmixStateType.Play;

    public bool IsPrimaryAnimationData { get; set; }

    public int FrameCount { get; set; }
    public int InitialFrame { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }

    public List<AnimationTriggerData> AnimationTriggers { get; } = [];

    public Texture2D? TryGetOrLoadGadgetTexture(
        StyleIdentifier styleIdentifier,
        PieceIdentifier gadgetPieceIdentifier)
    {
        if (TextureFilePath is null)
            return null;

        // Need to load the texture here to get its dimensions, since that data is not present in the NeoLemmix config file
        return TextureCache.GetOrLoadTexture(TextureFilePath, styleIdentifier, gadgetPieceIdentifier, TextureType.GadgetSprite);
    }
}
