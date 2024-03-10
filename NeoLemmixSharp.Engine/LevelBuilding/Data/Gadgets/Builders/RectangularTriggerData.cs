namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public readonly struct RectangularTriggerData
{
    public required int TriggerX { get; init; }
    public required int TriggerY { get; init; }
    public required int TriggerWidth { get; init; }
    public required int TriggerHeight { get; init; }
}