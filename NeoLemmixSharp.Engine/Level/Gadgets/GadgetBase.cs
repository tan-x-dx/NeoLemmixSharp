using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, IRectangularBounds, ISnapshotDataConvertible<int>
{
    public int Id { get; }
    public abstract GadgetBehaviour GadgetBehaviour { get; }
    public abstract Orientation Orientation { get; }
    public RectangularHitBoxRegion GadgetBounds { get; }
    public IGadgetRenderer? Renderer { get; }

    public LevelPosition TopLeftPixel { get; set; }
    public LevelPosition BottomRightPixel { get; set; }
    public LevelPosition PreviousTopLeftPixel { get; set; }
    public LevelPosition PreviousBottomRightPixel { get; set; }

    protected GadgetBase(
        int id,
        RectangularHitBoxRegion gadgetBounds,
        IGadgetRenderer? renderer)
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
    public void ToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
    }
}