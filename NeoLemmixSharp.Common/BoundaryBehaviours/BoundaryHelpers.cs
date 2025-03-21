using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public static class BoundaryHelpers
{
    [DoesNotReturn]
    private static BoundaryBehaviour ThrowUnknownBoundaryBehaviourException(BoundaryBehaviourType boundaryBehaviourType)
    {
        throw new ArgumentOutOfRangeException(
            nameof(boundaryBehaviourType),
            boundaryBehaviourType,
            "Unknown boundary behaviour type");
    }

    public static BoundaryBehaviour GetHorizontalBoundaryBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelWidth)
    {
        if (boundaryBehaviourType is BoundaryBehaviourType.Void or BoundaryBehaviourType.Wrap)
            return new BoundaryBehaviour(
                DimensionType.Horizontal,
                boundaryBehaviourType,
                levelWidth);

        return ThrowUnknownBoundaryBehaviourException(boundaryBehaviourType);
    }

    public static BoundaryBehaviour GetVerticalBoundaryBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelHeight)
    {
        if (boundaryBehaviourType is BoundaryBehaviourType.Void or BoundaryBehaviourType.Wrap)
            return new BoundaryBehaviour(
                DimensionType.Vertical,
                boundaryBehaviourType,
                levelHeight);

        return ThrowUnknownBoundaryBehaviourException(boundaryBehaviourType);
    }
}