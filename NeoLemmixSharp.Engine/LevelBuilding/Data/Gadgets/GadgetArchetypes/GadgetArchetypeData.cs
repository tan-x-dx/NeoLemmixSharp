using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.GadgetArchetypes;

public abstract class GadgetArchetypeData
{
    public abstract GadgetBase CreateGadget(int id, GadgetDataAaa gadgetData);
}