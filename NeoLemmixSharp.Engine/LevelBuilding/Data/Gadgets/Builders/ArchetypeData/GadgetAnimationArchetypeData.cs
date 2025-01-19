using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class GadgetAnimationArchetypeData
{
    public required int SpriteWidth { get; init; }
    public required int SpriteHeight { get; init; }
    public required int Layer { get; init; }
    public required int InitialFrame { get; init; }
    public required int MinFrame { get; init; }
    public required int MaxFrame { get; init; }
    public required GadgetSecondaryAnimationAction SecondaryAnimationAction { get; init; }

    public GadgetStateAnimationBehaviour GetAnimationBehaviour()
    {
        return new GadgetStateAnimationBehaviour(
            SpriteWidth,
            SpriteHeight,
            Layer,
            InitialFrame,
            MinFrame,
            MaxFrame,
            SecondaryAnimationAction);
    }
}