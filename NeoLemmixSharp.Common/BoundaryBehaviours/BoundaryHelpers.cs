using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public static class BoundaryHelpers
{
    public static IHorizontalViewPortBehaviour GetHorizontalViewPortBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int levelWidth) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new HorizontalVoidViewPortBehaviour(levelWidth),
        //   BoundaryBehaviourType.Solid => expr,
        //   BoundaryBehaviourType.Reflect => expr,
        BoundaryBehaviourType.Wrap => new HorizontalWrapViewPortBehaviour(levelWidth),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IVerticalViewPortBehaviour GetVerticalViewPortBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int levelHeight) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new VerticalVoidViewPortBehaviour(levelHeight),
        //   BoundaryBehaviourType.Solid => expr,
        //   BoundaryBehaviourType.Reflect => expr,
        BoundaryBehaviourType.Wrap => new VerticalWrapViewPortBehaviour(levelHeight),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IHorizontalBoundaryBehaviour GetHorizontalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int levelWidth) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Wrap => new HorizontalWrapBoundaryBehaviour(levelWidth),
        BoundaryBehaviourType.Void => new HorizontalVoidBoundaryBehaviour(levelWidth),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IVerticalBoundaryBehaviour GetVerticalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int levelHeight) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Wrap => new VerticalWrapBoundaryBehaviour(levelHeight),
        BoundaryBehaviourType.Void => new VerticalVoidBoundaryBehaviour(levelHeight),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };
}