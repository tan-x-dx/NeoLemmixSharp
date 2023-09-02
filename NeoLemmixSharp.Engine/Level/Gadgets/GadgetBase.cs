using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>
{
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

    public abstract void ReactToInput(string inputName, int payload);

    public abstract bool CaresAboutLemmingInteraction { get; }
    public abstract bool MatchesLemming(Lemming lemming);
    public abstract bool MatchesPosition(LevelPosition levelPosition);

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;
}