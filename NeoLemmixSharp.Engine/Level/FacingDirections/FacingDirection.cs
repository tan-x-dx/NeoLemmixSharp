using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class FacingDirection : IExtendedEnumType<FacingDirection>
{
    public static readonly FacingDirection LeftInstance = new(
        LevelConstants.LeftFacingDirectionId,
        LevelConstants.LeftFacingDirectionDeltaX,
        LevelConstants.LeftFacingDirectionName);

    public static readonly FacingDirection RightInstance = new(
        LevelConstants.RightFacingDirectionId,
        LevelConstants.RightFacingDirectionDeltaX,
        LevelConstants.RightFacingDirectionName);

    private static readonly FacingDirection[] FacingDirections = GenerateFacingDirectionCollection();

    public static int NumberOfItems => FacingDirections.Length;
    public static ReadOnlySpan<FacingDirection> AllItems => new(FacingDirections);

    private static FacingDirection[] GenerateFacingDirectionCollection()
    {
        var facingDirections = new FacingDirection[2];

        facingDirections[LeftInstance.Id] = LeftInstance;
        facingDirections[RightInstance.Id] = RightInstance;

        // No need for id validation here. It's just that simple

        return facingDirections;
    }

    public readonly int Id;
    public readonly int DeltaX;

    private readonly string _name;

    private FacingDirection(int id, int deltaX, string name)
    {
        Id = id;
        DeltaX = deltaX;
        _name = name;
    }

    [Pure]
    public FacingDirection GetOpposite()
    {
        return FacingDirections[1 - Id];
    }

    [Pure]
    public Orientation ConvertToRelativeOrientation(Orientation orientation)
    {
        return Orientation.Rotate(orientation, DeltaX);
    }

    int IIdEquatable<FacingDirection>.Id => Id;

    public bool Equals(FacingDirection? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public override int GetHashCode() => Id;
    public override string ToString() => _name;

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.Id == right.Id;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.Id != right.Id;
}