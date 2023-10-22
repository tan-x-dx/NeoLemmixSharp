using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public abstract class GadgetArchetypeData
{
    public abstract GadgetBase CreateGadget(int id, GadgetData gadgetData);
}