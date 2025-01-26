using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class FacingDirection : IExtendedEnumType<FacingDirection>
{
    public static readonly FacingDirection Left = new(
        EngineConstants.LeftFacingDirectionId,
        EngineConstants.LeftFacingDirectionDeltaX,
        EngineConstants.LeftFacingDirectionName);

    public static readonly FacingDirection Right = new(
        EngineConstants.RightFacingDirectionId,
        EngineConstants.RightFacingDirectionDeltaX,
        EngineConstants.RightFacingDirectionName);

    private static readonly FacingDirection[] FacingDirections = GenerateFacingDirectionCollection();

    public static int NumberOfItems => FacingDirections.Length;
    public static ReadOnlySpan<FacingDirection> AllItems => new(FacingDirections);

    private static FacingDirection[] GenerateFacingDirectionCollection()
    {
        var facingDirections = new FacingDirection[2];

        facingDirections[Left.Id] = Left;
        facingDirections[Right.Id] = Right;

        // No need for id validation here. It's just that simple

        return facingDirections;
    }

    private readonly string _name;

    public readonly int Id;
    public readonly int DeltaX;

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
        return orientation.Rotate(-DeltaX);
    }

    int IIdEquatable<FacingDirection>.Id => Id;

    public bool Equals(FacingDirection? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public override int GetHashCode() => Id;
    public override string ToString() => _name;

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.Id == right.Id;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.Id != right.Id;
}