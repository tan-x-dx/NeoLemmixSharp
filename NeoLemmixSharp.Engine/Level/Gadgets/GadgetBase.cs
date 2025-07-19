using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible<int>
{
    private readonly string _gadgetName;
    public required GadgetBounds CurrentGadgetBounds { get; init; }

    public required int Id { get; init; }
    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool IsFastForward { get; init; }

    public Point Position => CurrentGadgetBounds.Position;
    public Size Size => CurrentGadgetBounds.Size;

    public abstract GadgetState CurrentState { get; }

    public GadgetBase(string gadgetName)
    {
        _gadgetName = gadgetName;
    }

    public abstract void Tick();
    public abstract void SetNextState(int stateIndex);

    public bool Equals(GadgetBase? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;

    public void WriteToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
    }

    public sealed override string ToString() => _gadgetName;
}
