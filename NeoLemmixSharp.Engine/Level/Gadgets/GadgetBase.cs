using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible
{
    private readonly int _requiredNumberOfBytesForSnapshotting;
    public required GadgetName GadgetName { get; init; }
    public GadgetType GadgetType { get; }
    public required GadgetBounds CurrentGadgetBounds { get; init; }

    public required int Id { get; init; }
    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool IsFastForward { get; init; }

    public Point Position => CurrentGadgetBounds.Position;
    public Size Size => CurrentGadgetBounds.Size;

    public abstract GadgetState CurrentState { get; }

    protected GadgetBase(GadgetType gadgetType)
    {
        GadgetType = gadgetType;
        _requiredNumberOfBytesForSnapshotting = CalculateRequiredNumberOfBytesForSnapshotting();
    }

    public abstract void Tick();

    public bool Equals(GadgetBase? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;

    private int CalculateRequiredNumberOfBytesForSnapshotting()
    {
        return 1;
    }

    public int GetRequiredNumberOfBytesForSnapshotting() => _requiredNumberOfBytesForSnapshotting;

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        *snapshotDataPointer = 0;
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
    }

    public sealed override string ToString() => GadgetName.ToString();
}
