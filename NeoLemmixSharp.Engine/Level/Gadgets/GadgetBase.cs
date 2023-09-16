using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, IRectangularBounds
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

    public LevelPosition TopLeftPixel { get; protected set; }
    public LevelPosition BottomRightPixel { get; protected set; }
    public LevelPosition PreviousTopLeftPixel { get; protected set; }
    public LevelPosition PreviousBottomRightPixel { get; protected set; }

    protected GadgetBase(
        int id,
        RectangularLevelRegion gadgetBounds)
    {
        Id = id;
        GadgetBounds = gadgetBounds;

        TopLeftPixel = GadgetBounds.TopLeft;
        BottomRightPixel = GadgetBounds.BottomRight;

        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;
    }

    public abstract void Tick();

    protected void UpdatePosition(LevelPosition position)
    {
        PreviousTopLeftPixel = TerrainManager.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = TerrainManager.NormalisePosition(BottomRightPixel);

        position = TerrainManager.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = TerrainManager.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = TerrainManager.NormalisePosition(GadgetBounds.BottomRight);

        GadgetManager.UpdateGadgetPosition(this);
    }

    public abstract IGadgetInput? GetInputWithName(string inputName);

    public abstract bool CaresAboutLemmingInteraction { get; }
    public abstract bool MatchesLemming(Lemming lemming);
    public abstract bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition);
    public abstract bool MatchesPosition(LevelPosition levelPosition);

    public abstract void OnLemmingMatch(Lemming lemming);

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;
}