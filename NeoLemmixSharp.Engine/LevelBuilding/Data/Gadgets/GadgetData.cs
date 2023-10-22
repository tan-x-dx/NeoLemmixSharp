using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public abstract class GadgetData
{
    public GadgetArchetypeData ArchetypeData { get; init; } = null!;

    public int X { get; set; }
    public int Y { get; set; }

    public abstract GadgetBase CreateGadget();
}