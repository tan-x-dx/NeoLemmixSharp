using System.Diagnostics.CodeAnalysis;
using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public static class BoundaryHelpers
{
    [DoesNotReturn]
    private static T ThrowUnknownBoundaryBehaviourException<T>(BoundaryBehaviourType boundaryBehaviourType)
    {
        throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type");
    }

    public static IHorizontalViewPortBehaviour GetHorizontalViewPortBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelWidth) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new HorizontalVoidViewPortBehaviour(levelWidth),
        BoundaryBehaviourType.Wrap => new HorizontalWrapViewPortBehaviour(levelWidth),

        _ => ThrowUnknownBoundaryBehaviourException<IHorizontalViewPortBehaviour>(boundaryBehaviourType)
    };

    public static IVerticalViewPortBehaviour GetVerticalViewPortBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelHeight) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new VerticalVoidViewPortBehaviour(levelHeight),
        BoundaryBehaviourType.Wrap => new VerticalWrapViewPortBehaviour(levelHeight),

        _ => ThrowUnknownBoundaryBehaviourException<IVerticalViewPortBehaviour>(boundaryBehaviourType)
    };

    public static IHorizontalBoundaryBehaviour GetHorizontalBoundaryBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelWidth) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Wrap => new HorizontalWrapBoundaryBehaviour(levelWidth),
        BoundaryBehaviourType.Void => new HorizontalVoidBoundaryBehaviour(levelWidth),

        _ => ThrowUnknownBoundaryBehaviourException<IHorizontalBoundaryBehaviour>(boundaryBehaviourType)
    };

    public static IVerticalBoundaryBehaviour GetVerticalBoundaryBehaviour(
        this BoundaryBehaviourType boundaryBehaviourType,
        int levelHeight) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Wrap => new VerticalWrapBoundaryBehaviour(levelHeight),
        BoundaryBehaviourType.Void => new VerticalVoidBoundaryBehaviour(levelHeight),

        _ => ThrowUnknownBoundaryBehaviourException<IVerticalBoundaryBehaviour>(boundaryBehaviourType)
    };
}