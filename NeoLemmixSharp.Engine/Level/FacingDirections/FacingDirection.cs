using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class FacingDirection : IExtendedEnumType<FacingDirection>
{
    public static readonly FacingDirection LeftInstance = new(
        LevelConstants.LeftFacingDirectionId,
        LevelConstants.LeftFacingDirectionDeltaX,
        "left",
        Orientation.RotateClockwise);

    public static readonly FacingDirection RightInstance = new(
        LevelConstants.RightFacingDirectionId,
        LevelConstants.RightFacingDirectionDeltaX,
        "right",
        Orientation.RotateCounterClockwise);

    private static readonly FacingDirection[] FacingDirections = GenerateFacingDirectionCollection();

    public static int NumberOfItems => FacingDirections.Length;
    public static ReadOnlySpan<FacingDirection> AllItems => new(FacingDirections);

    private static FacingDirection[] GenerateFacingDirectionCollection()
    {
        var facingDirections = new FacingDirection[2];

        facingDirections[LeftInstance.Id] = LeftInstance;
        facingDirections[RightInstance.Id] = RightInstance;

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<FacingDirection>(facingDirections));

        LeftInstance._opposite = RightInstance;
        RightInstance._opposite = LeftInstance;

        return facingDirections;
    }

    public readonly int Id;
    public readonly int DeltaX;

    private readonly string _name;
    private readonly Func<Orientation, Orientation> _rotate;
    private FacingDirection _opposite;

    private FacingDirection(int id, int deltaX, string name, Func<Orientation, Orientation> rotate)
    {
        Id = id;
        DeltaX = deltaX;
        _name = name;
        _rotate = rotate;
    }

    int IIdEquatable<FacingDirection>.Id => Id;

    [Pure]
    public FacingDirection GetOpposite() => _opposite;
    [Pure]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => _rotate(orientation);

    public bool Equals(FacingDirection? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public override int GetHashCode() => Id;
    public override string ToString() => _name;

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.Id == right.Id;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.Id != right.Id;
}