using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>
{
    public int Id { get; }
    public abstract GadgetBehaviour GadgetBehaviour { get; }
    public abstract Orientation Orientation { get; }
    public RectangularLevelRegion GadgetBounds { get; }
    public IViewportObjectRenderer? Renderer { get; }

    protected GadgetBase(
        int id,
        RectangularLevelRegion gadgetBounds,
        IViewportObjectRenderer? renderer)
    {
        Id = id;
        GadgetBounds = gadgetBounds;
        Renderer = renderer;
    }

    public abstract void Tick();

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;
}