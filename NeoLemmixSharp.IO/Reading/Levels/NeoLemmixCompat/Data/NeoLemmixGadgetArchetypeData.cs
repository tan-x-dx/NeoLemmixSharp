using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

internal sealed class NeoLemmixGadgetArchetypeData
{
    public StyleIdentifier StyleIdentifier { get; }
    public PieceIdentifier GadgetPieceIdentifier { get; }

    public NeoLemmixGadgetArchetypeData(StyleIdentifier styleIdentifier, PieceIdentifier gadgetPieceIdentifier)
    {
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

    public ResizeType ResizeType { get; set; }

    public bool IsSkillPickup { get; set; }

    public List<NeoLemmixGadgetAnimationData> AnimationData { get; } = new();
}
