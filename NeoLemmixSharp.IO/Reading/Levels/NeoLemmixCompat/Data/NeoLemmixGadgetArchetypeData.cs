using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

internal sealed class NeoLemmixGadgetArchetypeData
{
    public string FilePath { get; }
    public StyleIdentifier StyleIdentifier { get; }
    public PieceIdentifier GadgetPieceIdentifier { get; }

    public NeoLemmixGadgetArchetypeData(
        string filePath,
        StyleIdentifier styleIdentifier,
        PieceIdentifier gadgetPieceIdentifier)
    {
        FilePath = filePath;
        StyleIdentifier = styleIdentifier;
        GadgetPieceIdentifier = gadgetPieceIdentifier;
    }

    public NeoLemmixGadgetBehaviour Behaviour { get; set; }

    public int SpriteWidth { get; set; } = -1;
    public int SpriteHeight { get; set; } = -1;

    public bool HasSpriteSizeSpecified => SpriteWidth >= 0 && SpriteHeight >= 0;

    public int TriggerX { get; set; }
    public int TriggerY { get; set; }

    public int TriggerWidth { get; set; } = -1;
    public int TriggerHeight { get; set; } = -1;

    public bool HasTriggerSizeSpecified => TriggerWidth >= 0 && TriggerHeight >= 0;

    public int DefaultWidth { get; set; } = -1;
    public int DefaultHeight { get; set; } = -1;
    public bool HasDefaultSizeSpecified => DefaultWidth >= 0 && DefaultHeight >= 0;

    public ResizeType ResizeType { get; set; }

    public bool IsSkillPickup { get; set; }

    public List<NeoLemmixGadgetAnimationData> AnimationData { get; } = new();

    public void GetOrLoadGadgetTexture(out string pngFilePath, out Texture2D texture)
    {
        // Need to load the texture here to get its dimensions, since that data is not present in the NeoLemmix config file
        pngFilePath = RootDirectoryManager.GetCorrespondingImageFile(FilePath);
        texture = TextureCache.GetOrLoadTexture(pngFilePath, StyleIdentifier, GadgetPieceIdentifier, TextureType.GadgetSprite);
    }
}
