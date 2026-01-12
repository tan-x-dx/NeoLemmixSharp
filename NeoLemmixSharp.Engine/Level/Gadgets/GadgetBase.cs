using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IEquatable<GadgetBase>
{
    private readonly PointerWrapper _stateIndex;
    public required nint DataHandle
    {
        init => _stateIndex = new PointerWrapper(value);
    }
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

    protected ref int StateIndex => ref _stateIndex.IntValue;

    protected GadgetBase(
        GadgetType gadgetType)
    {
        GadgetType = gadgetType;
    }

    public abstract void Tick();
    public abstract void SetState(int stateIndex);

    public bool Equals(GadgetBase? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase? left, GadgetBase? right)
    {
        var leftValue = -1;
        if (left is not null) leftValue = left.Id;
        var rightValue = -1;
        if (right is not null) rightValue = right.Id;
        return leftValue == rightValue;
    }
    public static bool operator !=(GadgetBase? left, GadgetBase? right) => !(left == right);

    public sealed override string ToString() => GadgetName.ToString();
}
