using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>
{
    protected static TerrainManager TerrainManager { get; private set; } = null!;
    protected static LemmingManager LemmingManager { get; private set; } = null!;
    protected static GadgetManager GadgetManager { get; private set; } = null!;

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        TerrainManager = terrainManager;
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        LemmingManager = lemmingManager;
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
        GadgetManager = gadgetManager;
    }

    public int Id { get; }
    public abstract GadgetType Type { get; }
    public abstract Orientation Orientation { get; }
    public RectangularLevelRegion GadgetBounds { get; }

    protected GadgetBase(
        int id,
        RectangularLevelRegion gadgetBounds)
    {
        Id = id;
        GadgetBounds = gadgetBounds;
    }

    public abstract void Tick();

    public abstract IGadgetInput? GetInputWithName(string inputName);

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;
}