using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public static class BoundaryHelpers
{
    public static BoundaryBehaviour GetHorizontalBoundaryBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelWidth)
    {
        if (boundaryBehaviourType is BoundaryBehaviourType.Void or BoundaryBehaviourType.Wrap)
            return new BoundaryBehaviour(
                DimensionType.Horizontal,
                boundaryBehaviourType,
                levelWidth);

        return Helpers.ThrowUnknownEnumValueException<BoundaryBehaviourType, BoundaryBehaviour>(boundaryBehaviourType);
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

        return Helpers.ThrowUnknownEnumValueException<BoundaryBehaviourType, BoundaryBehaviour>(boundaryBehaviourType);
    }
}
