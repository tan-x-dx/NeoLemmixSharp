using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
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

    public abstract IGadgetInput? GetInputWithName(string inputName);

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;
}