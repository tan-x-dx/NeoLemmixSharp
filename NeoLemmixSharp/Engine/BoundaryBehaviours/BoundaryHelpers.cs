using System;
using NeoLemmixSharp.Engine.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.BoundaryBehaviours.Vertical;

namespace NeoLemmixSharp.Engine.BoundaryBehaviours;

public static class BoundaryHelpers
{
    public static IHorizontalViewPortBehaviour GetHorizontalViewPortBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new HorizontalVoidViewPortBehaviour(width),
        //   BoundaryBehaviourType.Solid => expr,
        //   BoundaryBehaviourType.Reflect => expr,
        BoundaryBehaviourType.Wrap => new HorizontalWrapBehaviour(width),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IVerticalViewPortBehaviour GetVerticalViewPortBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int height) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new VerticalVoidViewPortBehaviour(height),
        //   BoundaryBehaviourType.Solid => expr,
        //   BoundaryBehaviourType.Reflect => expr,
        BoundaryBehaviourType.Wrap => new VerticalWrapViewPortBehaviour(height),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IHorizontalBoundaryBehaviour GetHorizontalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Wrap => new HorizontalWrapBoundaryBehaviour(width),
        BoundaryBehaviourType.Void => new HorizontalVoidBoundaryBehaviour(),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IVerticalBoundaryBehaviour GetVerticalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int height) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Wrap => new VerticalWrapBoundaryBehaviour(height),
        BoundaryBehaviourType.Void => new VerticalVoidBoundaryBehaviour(),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };
}