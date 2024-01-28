namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class GadgetArchetypeData : IStyleArchetypeData
{
    public required int GadgetArchetypeId { get; init; }

    public required string? Style { get; init; }
    string? IStyleArchetypeData.Piece => Gadget;
    public required string? Gadget { get; init; }
}